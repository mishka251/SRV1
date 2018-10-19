using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace SRV1
{
    public partial class Form1 : Form
    {

        Mutex mutex;
        Queue<int> queue;
        bool work = true;
        public Form1()
        {
            InitializeComponent();
            r = new Random();
            t1 = new Thread(new ThreadStart(tr1));
            t2 = new Thread(new ThreadStart(tr2));

            mutex = new Mutex();
            queue = new Queue<int>();
        }

        Random r;
        int GetNum()
        {
            return r.Next(1, 9);
        }

        void tr1()
        {
            while (work)
            {
                int num = GetNum();

                mutex.WaitOne();
                queue.Enqueue(num);
                int[] arr= new int[queue.Count];
                queue.CopyTo(arr, 0);
                lbBuf.Invoke(new Action(() => lbBuf.Items.Add(num.ToString())));
                
                mutex.ReleaseMutex();

                          
                Thread.Sleep(r.Next(1000, 2000));
            }
        }

        void tr2()
        {
            while (work)
            {

                mutex.WaitOne();
                bool has_elem = queue.Count>0;
                int num=0;
                if (has_elem)
                {
                    num = queue.Dequeue();
                    lbBuf.Invoke(new Action(() => lbBuf.Items.RemoveAt(0)));
                }
                mutex.ReleaseMutex();
                
                lblNow.BeginInvoke(new Action(() => lblNow.Text = (has_elem ? num.ToString() : "")));
                Thread.Sleep(r.Next(1500, 2500));
            }
        }


        Thread t1;
        Thread t2;



        private void btnStart_Click(object sender, EventArgs e)
        {
            t1.Start();
            t2.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            work = false;
            Thread.Sleep(300);
           // Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            work = false;           
            Thread.Sleep(2500);
        }
    }
}
