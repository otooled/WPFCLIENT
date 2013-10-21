using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Win32;
using HundredMilesSoftware.UltraID3Lib;
using System.IO;


namespace SelecTunes_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer mp;
        DispatcherTimer clock = new DispatcherTimer();
        UltraID3 ID3 = new UltraID3();
        List<Track> tracks = new List<Track>();
        Random rnd = new Random();
        Track currentTrack;
        Track nextTrack;
        TimeSpan trackDuration;
        private string trackLength;
        private string thirtySeconds = "00:00:30";
        int trackNo = 0;
        private double stage = 0;
        private TimeSpan timeLeft;
        private bool flag = true;
        List<Track> qeuedTracks = new List<Track>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mp = new MediaPlayer();
            mp.MediaEnded += mp_MediaEnded;
            clock.Interval = new TimeSpan(0,0,0,1);
            clock.Tick += clock_Tick;
            trackDuration = new TimeSpan();
        }

        void mp_MediaEnded(object sender, EventArgs e)
        {
            TbkNextTrack.Text = "";
            PrgBarTrack.Value = 0;
            clock.Stop();
            currentTrack = nextTrack;
            nextTrack = (Track) LbxQeuedTracks.Items[0];
            TbkCurrentSong.Text = currentTrack.Name;
            PrgBarTrack.Maximum = currentTrack.Duration.TotalSeconds;
            TbkNextTrack.Text = nextTrack.Name;
            mp.Open(new Uri(currentTrack.Location));
            trackDuration = currentTrack.Duration;
            mp.Play();
            clock.Start();
            flag = true;
        }

        void clock_Tick(object sender, EventArgs e)
        {
            if (LbxQeuedTracks.Items.Count < 5)
            {
                trackNo = rnd.Next(LbxTracksUpload.Items.Count);
                LbxQeuedTracks.Items.Add(LbxTracksUpload.Items[trackNo]);
            }
            if(LbxQeuedTracks.Items.Contains(currentTrack))
            {
                LbxQeuedTracks.Items.Remove(currentTrack);
            }
            if(LbxQeuedTracks.Items.Contains(nextTrack))
            {
                LbxQeuedTracks.Items.Remove(nextTrack);
            }
            trackDuration = trackDuration.Subtract(new TimeSpan(0, 0, 0, 1));
            TbkSeconds.Text = trackDuration.ToString();
            PrgBarTrack.Value += PrgBarTrack.Maximum - (PrgBarTrack.Maximum - 1);
            if ((trackDuration <= TimeSpan.Parse(thirtySeconds)) && flag)
            {
                trackNo = rnd.Next(LbxTracksUpload.Items.Count);
                //nextTrack = (Track) LbxQeuedTracks.Items[0];
                //TbkNextTrack.Text = nextTrack.Name;
                flag = false;
                LbxQeuedTracks.Items.Remove(nextTrack);
            }
            
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (currentTrack == null)
            {
                if(LbxTracksUpload.Items.Count > 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        trackNo = rnd.Next(LbxTracksUpload.Items.Count);
                        qeuedTracks.Add((Track)LbxTracksUpload.Items[trackNo]);
                    }
                    foreach (Track qeuedTrack in qeuedTracks)
                    {
                        LbxQeuedTracks.Items.Add(qeuedTrack);
                    }
                    //trackNo = rnd.Next(LbxTracksUpload.Items.Count);
                    //currentTrack = (Track)LbxTracksUpload.Items[trackNo];
                    currentTrack = (Track)LbxQeuedTracks.Items[0];
                    nextTrack = (Track) LbxQeuedTracks.Items[1];
                    LbxQeuedTracks.Items.Remove(currentTrack);
                    LbxQeuedTracks.Items.Remove(nextTrack);
                    TbkCurrentSong.Text = currentTrack.Name;
                    TbkNextTrack.Text = nextTrack.Name;
                    mp.Open(new Uri(currentTrack.Location));
                    trackDuration = currentTrack.Duration;
                    PrgBarTrack.Maximum = trackDuration.TotalSeconds;
                    PrgBarTrack.Value = 0;
                    mp.Play();
                    clock.Start();
                    //comment in play method
                }
                else
                {
                    MessageBox.Show("No Tracks have been added");
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    trackNo = rnd.Next(LbxTracksUpload.Items.Count);
                    qeuedTracks.Add((Track)LbxTracksUpload.Items[trackNo]);
                }
                foreach (Track qeuedTrack in qeuedTracks)
                {
                    LbxQeuedTracks.Items.Add(qeuedTrack);
                }
                //trackNo = rnd.Next(LbxTracksUpload.Items.Count);
                //currentTrack = (Track) LbxTracksUpload.Items[trackNo];
                currentTrack = (Track)LbxQeuedTracks.Items[0];
                LbxQeuedTracks.Items.Remove(currentTrack);
                TbkCurrentSong.Text = currentTrack.Name;
                mp.Open(new Uri(currentTrack.Location));
                trackDuration = currentTrack.Duration;
                PrgBarTrack.Maximum = trackDuration.TotalSeconds;
                PrgBarTrack.Value = 0;
                mp.Play();
                clock.Start();
            }
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            mp.Pause();
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            mp.Stop();
        }

        private void BtnAddTracks_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Load Song";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == true)
            {
                foreach (string trac in ofd.FileNames)
                {
                    Track t = new Track();
                    ID3.Read(trac);
                    t.Album = ID3.Album;
                    t.Artist = ID3.Artist;
                    t.Duration = ID3.Duration;
                    t.Location = Path.GetFullPath(trac);
                    t.Name = ID3.Title;
                    tracks.Add(t);
                }
            }

            foreach (Track t in tracks)
            {
                LbxTracksUpload.Items.Add(t);
            }
        }

        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LbxTracksUpload_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox l = (ListBox) sender;
            Track t = (Track) l.SelectedItem;
            TbkAlbum.Text = t.Album;
            TbkArtist.Text = t.Artist;
        }
        
    }
}
