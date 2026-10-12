using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;

class RainForm : Form {
    public static readonly string[] Code = Array.ConvertAll(
        "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz{}[]()<>=+-*/;:_&|!#$%".ToCharArray(), c=>c.ToString());
    class Stream { public float X,Y,Speed,Size,Clock; public int Length; public float Depth; public string[] Chars; }
    readonly List<Stream> streams = new List<Stream>();
    readonly Random rng = new Random(2026);
    readonly Font font = new Font("Consolas", 20, FontStyle.Regular, GraphicsUnit.Pixel);
    readonly Font farFont = new Font("Consolas", 14, FontStyle.Regular, GraphicsUnit.Pixel);
    readonly Timer timer = new Timer();
    readonly bool preview, manual;
    readonly DateTime started = DateTime.UtcNow;
    Point initialMouse;
    float speed = 1; bool paused, cursorHidden;
    [DllImport("user32.dll")] static extern IntPtr SetParent(IntPtr child, IntPtr parent);
    [DllImport("user32.dll")] static extern int SetWindowLong(IntPtr handle,int index,int value);
    [DllImport("user32.dll")] static extern bool GetClientRect(IntPtr handle,out RECT rect);
    [StructLayout(LayoutKind.Sequential)] struct RECT {public int Left,Top,Right,Bottom;}
    public RainForm(Rectangle bounds, bool isManual, IntPtr parent) {
        manual=isManual;preview=parent!=IntPtr.Zero;
        FormBorderStyle=FormBorderStyle.None; ShowInTaskbar=false;
        BackColor=Color.FromArgb(1,5,3); DoubleBuffered=true; KeyPreview=true;
        Bounds=bounds; StartPosition=FormStartPosition.Manual; TopMost=!preview;
        if(preview) { SetWindowLong(Handle,-16,0x50000000);SetParent(Handle,parent); RECT r;GetClientRect(parent,out r);Bounds=new Rectangle(0,0,r.Right,r.Bottom); }
        ResetStreams(); initialMouse=Cursor.Position;
        timer.Interval=33; timer.Tick+=(s,e)=>{ if(!paused)Advance(.033f);Invalidate(); };timer.Start();
        KeyDown+=(s,e)=>{ HandleKey(e.KeyCode);e.Handled=true; };
        MouseDown+=(s,e)=>{if(!preview)Application.Exit();};
        MouseMove+=(s,e)=>{if(!manual&&!preview&&(DateTime.UtcNow-started).TotalSeconds>2 && (Math.Abs(Cursor.Position.X-initialMouse.X)>8||Math.Abs(Cursor.Position.Y-initialMouse.Y)>8))Application.Exit();};
        Shown+=(s,e)=>{if(!preview){Cursor.Hide();cursorHidden=true;}};
        FormClosed+=(s,e)=>{if(cursorHidden)Cursor.Show();timer.Dispose();font.Dispose();farFont.Dispose();};
    }
    void ResetStreams() {
        streams.Clear();
        for(int layer=0;layer<2;layer++) {
            float size=layer==0?14:20, depth=layer==0?.45f:1f;
            for(float x=0;x<Width;x+=size*1.1f) {
                int length=10+rng.Next(25);
                string[] chars=new string[length];
                for(int i=0;i<length;i++)chars[i]=Code[rng.Next(Code.Length)];
                streams.Add(new Stream{X=x,Y=(float)rng.NextDouble()*(Height+length*size),Size=size,Speed=(45+rng.Next(100))*depth,Length=length,Chars=chars,Depth=depth});
            }
        }
    }
    public void Advance(float dt) {
        foreach(Stream s in streams){
            s.Y+=dt*s.Speed*speed;s.Clock+=dt;
            if(s.Clock>.09f){s.Clock=0;s.Chars[rng.Next(s.Length)]=Code[rng.Next(Code.Length)];}
            if(s.Y-s.Length*s.Size>Height){s.Y=-rng.Next(Math.Max(1,Height/2));s.Speed=(45+rng.Next(100))*s.Depth;}
        }
    }
    public void HandleKey(Keys key) {
        if(preview)return;
        if(key==Keys.Escape){Application.Exit();Close();return;}
        if(!manual){Application.Exit();return;}
        if(key==Keys.Space)paused=!paused;
        if(key==Keys.Up)speed=Math.Min(4,speed+.2f);
        if(key==Keys.Down)speed=Math.Max(.2f,speed-.2f);
    }
    public void Render(Graphics g) {
        g.Clear(BackColor);
        g.TextRenderingHint=System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        foreach(Stream s in streams)for(int j=s.Length-1;j>=0;j--){
            float y=s.Y-j*s.Size; if(y< -s.Size||y>Height)continue;
            float fade=(float)Math.Pow(1-(double)j/s.Length,1.5)*s.Depth;
            Color color=j==0&&s.Depth>.8?Color.FromArgb(220,255,232):Color.FromArgb((int)(22*fade),(int)(230*fade),(int)(92*fade));
            Font activeFont=s.Depth>.8?font:farFont;
            if(j==0&&s.Depth>.8)using(Brush glow=new SolidBrush(Color.FromArgb(45,90,255,150))) {
                g.DrawString(s.Chars[j],activeFont,glow,s.X-1,y);
                g.DrawString(s.Chars[j],activeFont,glow,s.X+1,y);
                g.DrawString(s.Chars[j],activeFont,glow,s.X,y-1);
            }
            using(Brush b=new SolidBrush(color))g.DrawString(s.Chars[j],activeFont,b,s.X,y);
        }
        if(manual&&(DateTime.UtcNow-started).TotalSeconds<5)
            using(Brush b=new SolidBrush(Color.FromArgb(160,220,180)))g.DrawString("ESC: close    SPACE: pause    UP/DOWN: speed",font,b,20,Height-35);
    }
    protected override void OnPaint(PaintEventArgs e) {Render(e.Graphics);}
    public static void SelfTest() {
        foreach(string line in Code) {
            if(line.Length!=1)throw new Exception("Expected single-character rain, not code lines");
            foreach(char c in line)if(c>127)throw new Exception("Non-ASCII code");
        }
        using(RainForm f=new RainForm(new Rectangle(0,0,1280,720),true,IntPtr.Zero)) {
            float before=f.streams[0].Y;f.Advance(.1f);if(f.streams[0].Y<=before)throw new Exception("Animation");
            f.HandleKey(Keys.Space);if(!f.paused)throw new Exception("Pause");f.HandleKey(Keys.Space);
            f.HandleKey(Keys.Up);if(f.speed<=1)throw new Exception("Speed");
            using(Bitmap bmp=new Bitmap(1280,720)){using(Graphics g=Graphics.FromImage(bmp))f.Render(g);bmp.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"native-preview.png"),ImageFormat.Png);}
            f.HandleKey(Keys.Escape);if(!f.IsDisposed)throw new Exception("ESC exit");
        }
        File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"native-test.txt"),"PASS: single ASCII characters, animation, pause, speed, render, ESC exit");
    }
}
class Program {
    [STAThread] static void Main(string[] args) {
        Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);
        string mode=args.Length==0?"/s":args[0].ToLowerInvariant();
        if(mode=="/test"){
            try { RainForm.SelfTest(); }
            catch(Exception e) { File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"native-test.txt"),"FAIL: "+e.Message);Environment.Exit(1); }
            return;
        }
        if(mode.StartsWith("/c")){MessageBox.Show("Matrix Code Rain\n\nESC closes manual mode. SPACE pauses, UP/DOWN adjust speed.\nChoose idle timeout and login protection in Windows Screen Saver Settings.","Matrix Code Rain");return;}
        if(mode.StartsWith("/p")){
            string value=mode.Contains(":")?mode.Substring(mode.IndexOf(':')+1):(args.Length>1?args[1]:"0");long n;
            if(long.TryParse(value,out n)&&n!=0)Application.Run(new RainForm(new Rectangle(0,0,320,200),false,new IntPtr(n)));return;
        }
        bool manual=mode=="/manual";
        using(System.Threading.Mutex mutex=new System.Threading.Mutex(false,"Local\\MatrixCodeRain")) {
            if(!mutex.WaitOne(0))return;
            try{foreach(Screen screen in Screen.AllScreens){RainForm f=new RainForm(screen.Bounds,manual,IntPtr.Zero);f.Show();}Application.Run();}
            finally{mutex.ReleaseMutex();}
        }
    }
}
