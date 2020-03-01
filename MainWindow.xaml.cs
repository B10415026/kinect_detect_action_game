//MultiaMedia_final
//于婷的腳趾是教官的腳跟(手要伸直)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
namespace KinectHand
{
    public partial class MainWindow : Window
    {
        KinectSensor sensor;
        ColorFrameReader colorFrameReader;
        BodyFrameReader bodyFrameReader;
        WriteableBitmap colorBitmap;
        Body[] bodies;
    }
    //to do
}

namespace multimedia  //專案名稱
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        KinectSensor kinect;
        BodyIndexFrameReader bodyIndexFrameReader;
        BodyFrameReader bodyFrameReader;
        Body[] bodies;
        Timer timer;
        int posture_start_time = -1;
        double initialDistance_hand_right_Z = 0;
        double initialDistance_Foot_right_Y = 0;
        double initialDistance_Hip_right_X = 0;
        double initialDistance_hand_left_Y = 0;
        double initialDistance_Hip_left_X = 0;
        bool fetch = false;
        bool hip_start = false;
        int Game_number = 10;
        int game_time = 60, story_time = 5,correct_time=2;
        int sec=5;       
        Random R_selectroad = new Random();
        int selectroad;
        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            //timer = new Timer();
            //timer.Interval = 1000;//1s
            //timer.Tick += new EventHandler(Timer_Tick);
            //timer.Start();

            //try
            //{
            //kinect設備獲取
            kinect = KinectSensor.GetDefault();
            kinect.Open();


            //開啟Reader
            bodyFrameReader = kinect.BodyFrameSource.OpenReader();
            bodyFrameReader.FrameArrived += Reader_BodyFrameArrived;
            Story.Visibility = Visibility.Visible;
            voice.Source = new Uri("C:\\Users\\Hua\\Desktop\\voice\\background.mp3");
            voice.Play();
            //創建一個顏色陣列 用來染色人體索引位置(最多總計6人)
            //bodyIndexColors = new Color[] { Colors.Red, Colors.Blue, Colors.Green, Colors.Yellow, Colors.Pink, Colors.Purple };

            timer = new Timer();
            timer.Interval = 1000;//1s
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
            

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    Close();
            //}
        }
       
        private void Timer_Tick(object sender, EventArgs e)
        {
            textBox.Text = "剩餘時間:" + sec;
            //超過時間未動作
            if (sec ==0 && posture_start_time == -1 && Game_number < 10)
            {
                sec = 60;
                posture_start_time = -1;
                Game_number = 40;
                LoadStory();
                //outputBox.Text = "over";
            }
            else if (sec == 0 && posture_start_time == -1 && Game_number < 20)
            {
                //textBox.Visibility = Visibility.Collapsed;
                sec = 5;
                posture_start_time = -1;
                LoadStory();
                //if (Game_number == 10)
                //{
                //    //Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\11.jpg");
                //    //Game_number = 11;

                //}
                //else
                //   // Game_number--;
            }
            else if(sec == 0 && posture_start_time == -1 && Game_number<30)
            {
                sec = 2;
                posture_start_time = -1;
                LoadStory();
                Game_number -= 9;

            }
            //else if(posture_start_time)
            else if (posture_start_time != -1)
            {
                posture_start_time = -1;
                //LoadStory(30);
            }
            else if (sec <= -10)
                timer.Stop();
            sec--;

        }
        //動作觸發進下一關

        //}
        //else
        //{
        //    voice.Stop();
        //    timer.Stop();
        //    textBox.Text = "Time's up";
        //}

        private void Reader_BodyFrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame == null)
                    return;
                if (bodies == null)
                {   //Create an array of the bodies in the scene and update it 
                    bodies = new Body[bodyFrame.BodyCount];
                }
                bodyFrame.GetAndRefreshBodyData(bodies);
                //For each body in the scene 
                foreach (Body body in bodies)
                {
                    if (body.IsTracked)
                    {
                        double story1, distance_hands_left_right;
                        double distance_head_Lefthand, distance_head_Righthand;
                        double distance_hands_left_right_X;
                        double distance_hand_right_Z;//推門動作
                        double distance_Foot_floor;//glico
                        double distance_hip_right;//選路
                        double distance_hip_left;//選路
                        double distance_lefthand;//自由女神像左手拿書
                        double distance_hand_right_Y;//自由女神拿冰淇淋
                        var joints = body.Joints;
                        // Get all of the joints in that body
                        if (joints[JointType.HandRight].TrackingState == TrackingState.Tracked && joints[JointType.HandLeft].TrackingState == TrackingState.Tracked && joints[JointType.FootLeft].TrackingState == TrackingState.Tracked && joints[JointType.FootRight].TrackingState == TrackingState.Tracked && joints[JointType.SpineMid].TrackingState == TrackingState.Tracked)
                        {

                            if (!fetch)
                            {
                                initialDistance_hand_right_Z = joints[JointType.HandRight].Position.Z;
                                initialDistance_Foot_right_Y = joints[JointType.FootLeft].Position.Y;
                                initialDistance_hand_left_Y = joints[JointType.HandLeft].Position.Y;
                                selectroad = R_selectroad.Next(0, 2);
                            }
                            fetch = true;
                            //X:左右(左右手各有其軸)  Y:上下(左右手各有其軸)   Z:前(0)後(Inf)
                            Right.Text = "Right " + (joints[JointType.HandLeft].Position.Y).ToString();
                            story1 = joints[JointType.HandLeft].Position.X - joints[JointType.HandRight].Position.X;
                            distance_hands_left_right = joints[JointType.HandLeft].Position.Y - joints[JointType.HandRight].Position.Y;
                            distance_head_Lefthand = joints[JointType.HandLeft].Position.X - joints[JointType.Head].Position.X;
                            distance_head_Righthand = joints[JointType.HandRight].Position.X - joints[JointType.Head].Position.X;
                            distance_hands_left_right_X = Math.Abs(joints[JointType.HandLeft].Position.X - joints[JointType.HandRight].Position.X);
                            distance_hand_right_Z = joints[JointType.HandRight].Position.Z - initialDistance_hand_right_Z;
                            distance_Foot_floor = Math.Abs(joints[JointType.FootLeft].Position.Y - initialDistance_Foot_right_Y);
                            distance_hip_right = joints[JointType.HipRight].Position.X - initialDistance_Hip_right_X;
                            distance_hip_left = joints[JointType.HipLeft].Position.X - initialDistance_Hip_left_X;
                            distance_lefthand = Math.Abs(joints[JointType.HandLeft].Position.Y - initialDistance_hand_left_Y);
                            distance_hand_right_Y = joints[JointType.HandRight].Position.Y;
                            Left.Text = "distance " + distance_hip_right+"/r/n" + distance_hip_left;
                            //distance_hands_left_right.ToString();
                            //姿勢 鐵達尼號 OK
                            if (Game_number == 1 && distance_hands_left_right_X > 1.30)
                            {
                                if (posture_start_time == -1)
                                {
                                    voice.Source = new Uri("C:\\Users\\Hua\\Desktop\\voice\\correct.mp3");
                                    voice.Play();
                                    Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\摩艾2.jpg");//答對的圖片
                                    outputBox.Text = "姿勢 鐵達尼號";
                                    posture_start_time = sec;
                                    Game_number=21;
                                    sec = correct_time;                                    
                                    //LoadStory();
                                }
                            }                       
                            //姿勢C OK
                            else if (Game_number == 2 && (distance_hands_left_right > 0.7) && (distance_head_Lefthand > 0.0) && (distance_head_Righthand > 0.0))
                            {
                                //Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\14.jpg");
                                if (posture_start_time == -1)
                                {
                                    voice.Source = new Uri("C:\\Users\\Hua\\Desktop\\voice\\correct.mp3");
                                    voice.Play();
                                    Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\YMCA2.jpg");
                                    outputBox.Text = "姿勢C";
                                    posture_start_time = sec;
                                    Game_number=22;
                                    sec = correct_time;                                    
                                    //LoadStory();
                                }
                            }
                            //姿勢選路start
                            else if (Game_number == 3 && hip_start == false)
                            {
                                initialDistance_Hip_right_X = joints[JointType.HipRight].Position.X;
                                initialDistance_Hip_left_X = joints[JointType.HipLeft].Position.X;
                                distance_hip_right = joints[JointType.HipRight].Position.X - initialDistance_Hip_right_X;
                                distance_hip_left = joints[JointType.HipLeft].Position.X - initialDistance_Hip_left_X;

                                hip_start = true;
                            }
                            //姿勢選路
                            else if (Game_number == 3 &&( (distance_hip_right > 0.20)|| (distance_hip_left < -0.15)))//hip distance還沒測
                            {
                               
                                if (posture_start_time == -1)
                                {
                                    outputBox.Text = "姿勢 選路";
                                    posture_start_time = sec;
                                    if (distance_hip_right > 0)
                                    {
                                        //慘阿
                                        Game_number = 30;
                                        sec = -10;
                                        LoadStory();

                                    }
                                    else if (distance_hip_right < 0)
                                    {
                                        voice.Source = new Uri("C:\\Users\\Hua\\Desktop\\voice\\correct.mp3");
                                        voice.Play();
                                        Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\03選擇題SUCCES.jpg");
                                        posture_start_time = sec;
                                        Game_number = 23;
                                        sec = correct_time;
                                        //LoadStory();
                                    }
                                }
                            }

                            //姿勢 glico(雙手高舉 右腳離地)
                            else if (Game_number ==4 && distance_hands_left_right_X > 0.7 && distance_Foot_floor > 0.35)
                            {
                                //Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\16.jpg");
                                if (posture_start_time == -1)
                                {
                                    voice.Source = new Uri("C:\\Users\\Hua\\Desktop\\voice\\correct.mp3");
                                    voice.Play();
                                    Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\glico2.jpg");
                                    outputBox.Text = "姿勢 glico";
                                    posture_start_time = sec;
                                    Game_number=24;
                                    sec = correct_time;
                                    //Story.Source = new Uri("C:\\Users\\user\\Desktop\\MultiaMedia_soource\\liberty.jpg");
                                   // LoadStory();
                                }
                            }
                            //姿勢自由女神(單手高舉) 
                            else if (Game_number ==5 && (distance_hand_right_Y > 0.75) && (distance_lefthand > 0.25))
                            {
                                //Story.Source = new Uri("C:\\Users\\user\\Desktop\\MultiaMedia_soource\\box.jpg");
                                if (posture_start_time == -1)
                                {
                                    voice.Source = new Uri("C:\\Users\\Hua\\Desktop\\voice\\correct.mp3");
                                    voice.Play();
                                    Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\自由女神糊2.jpg");//答對後正確的圖片
                                    outputBox.Text = "姿勢 自由女神";
                                    posture_start_time = sec;
                                    Game_number=25;
                                    sec = correct_time;
                                }
                            }
                            //選錯路 慘阿
                            else if (Game_number == 30)
                            {
                               // Story.Source = new Uri("C:\\Users\\user\\Desktop\\MultiaMedia_soource\\box.jpg");
                                //Game_number = 30;
                            }

                        }
                    }
                }
            }
        }
        private void LoadStory()
        {
            if (Game_number == 10&& posture_start_time == -1)
            {
                // sec++;
               
                Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\玩稿2_頁面_3.jpg");//故事1
                posture_start_time = sec;
                Game_number++;
            }
            else if (Game_number == 11)
            {
                Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\摩艾.jpg");//題目圖案
                voice.Source = new Uri("C:\\Users\\Hua\\Desktop\\voice\\voice01.mp3");
                voice.Play();
                posture_start_time = sec;
                sec = game_time;
                //Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\11.jpg");
                Game_number = 1;
            }
            //else if (Game_number == 12)
            //{
            //    Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\NARUTO.jpg");
            //    Game_number = 2;
            //}
            else if (Game_number == 12)
            {
                Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\YMCA1.jpg");
                voice.Source = new Uri("C:\\Users\\Hua\\Desktop\\voice\\voice02.mp3");
                voice.Play();
                posture_start_time = sec;
                sec = game_time;
                Game_number = 2;
            }
            else if (Game_number == 13)
            {
                voice.Stop();
                Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\選擇題.jpg");
                posture_start_time = sec;
                sec = game_time;
                Game_number = 3;
            }
            else if (Game_number == 14)
            {
                Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\glico1.jpg");
                voice.Source = new Uri("C:\\Users\\Hua\\Desktop\\voice\\voice04.mp3");
                voice.Play();
                posture_start_time = sec;
                sec = game_time;
                Game_number = 4;
            }
            else if (Game_number == 15)
            {
                Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\自由女神糊.jpg");
                voice.Source = new Uri("C:\\Users\\Hua\\Desktop\\voice\\voice05.mp3");
                voice.Play();
                posture_start_time = sec;
                sec = game_time;
                Game_number = 5;
            }
            else if (Game_number == 16)
            {
                Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\05結局.jpg");//問問題的題目
                sec = -10;
            }//世界之王的問題
            else if (Game_number == 25)//final success
            {
                Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\05結局.jpg");//勝利後的圖片
                sec = correct_time;
                // posture_start_time = sec;
            }
           
            else if (Game_number == 30)
            {
                Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\玩稿2_頁面_4.jpg");//選錯路的圖片
                voice.Source = new Uri("C:\\Users\\Hua\\Desktop\\voice\\wrong.mp3");
                voice.Play();
                //Story.Source = new Uri("C:\\Users\\user\\Desktop\\MultiaMedia_soource\\liberty.jpg");
                posture_start_time = sec;
            }//選錯路
            else if (Game_number == 40)
            {
                Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\gameover.jpg");//失敗的圖片
                voice.Source = new Uri("C:\\Users\\Hua\\Desktop\\voice\\wrong.mp3");
                voice.Play();
                sec = -10;

            }//失敗
            //else if (Game_number == 1)
            //{
            //    Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\12.jpg");
            //}//
            //else if (Game_number == 2)
            //{
            //    Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\13.jpg");
            //}
            //else if (Game_number == 3)
            //{
            //    Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\14.jpg");
            //}
            //else if (Game_number == 4)
            //{
            //    Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\15.jpg");
            //}
            //else if (Game_number == 5)
            //{
            //    Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\box.jpg");
            //}
            else if (Game_number == 21)//game 1 success
            {
                voice.Source = new Uri("C:\\Users\\Hua\\Desktop\\voice\\background.mp3");
                voice.Play();
                Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\story02.jpg");//故事2
                //Game_number = 12;
                sec = story_time;
            }
            else if (Game_number == 22)//game 2 success
            {
                voice.Source = new Uri("C:\\Users\\Hua\\Desktop\\voice\\background.mp3");
                voice.Play();
                Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\玩稿2_頁面_5.jpg");//叉路故事
                //Game_number = 13;
                sec = story_time;
            }
            else if (Game_number == 23)//intersection
            {
                voice.Source = new Uri("C:\\Users\\Hua\\Desktop\\voice\\background.mp3");
                voice.Play();
                Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\story03.jpg");//故事3
                //Game_number = 13;
                sec = story_time;
            }
            else if (Game_number == 24)//game 4 success
            {
                voice.Source = new Uri("C:\\Users\\Hua\\Desktop\\voice\\background.mp3");
                voice.Play();
                Story.Source = new Uri("C:\\Users\\Hua\\Desktop\\Story\\story04.jpg");//故事4
                // Game_number = 15;
                sec = story_time;
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (bodyIndexFrameReader != null)
            {
                bodyIndexFrameReader = null;
            }
            if (kinect != null)
            {
                kinect.Close();
                kinect = null;
            }
            if (bodyFrameReader != null)
            {
                bodyFrameReader = null;
            }
        }
    
    }
}