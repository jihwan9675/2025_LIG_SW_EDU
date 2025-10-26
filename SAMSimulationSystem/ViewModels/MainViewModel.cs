using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Input;
using SAMSimulationSystem.Commands;
using HelixToolkit.Wpf;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Data;


namespace SAMSimulationSystem.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged 
    {
        SAMSimulationWrapper.SAMSimulationWrapper _SAMSimulationWrapper;

        public MainViewModel()
        {
            _SAMSimulationWrapper = new SAMSimulationWrapper.SAMSimulationWrapper();

            InitCommands();

            // timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(dt);
            timer.Tick += Timer_Tick;

            // init visuals / defaults
            ResetSimulation();
        }

        void InitCommands() 
        {

            StartMissionCommand = new Command(StartMission);
            StopMissionCommand = new Command(StopMission);
            ResetMissionCommand = new Command(ResetMission);
        }

        #region Variable
        public ICommand StartMissionCommand { get; set; }
        public ICommand StopMissionCommand { get; set; }
        public ICommand ResetMissionCommand { get; set; }

        // 시뮬레이션 타이머
        private DispatcherTimer timer;
        private double dt = 0.04; // 25 fps-ish

        // Threat state
        private Point3D threatPos;
        private Vector3D threatVel;
        private double threatSpeed = 3.0;

        // Interceptor state
        private ModelVisual3D interceptorModel = null;
        private Point3D interceptorPos;
        private Vector3D interceptorVel;
        private double interceptorSpeed = 8.0;
        private List<Point3D> interceptorPath = new List<Point3D>();
        private bool interceptorLaunched = false;

        // Radar parameters
        private Point3D radarPos = new Point3D(0, 0, 0);
        private double RadarRange = 18.0;
        private double SweepAngleDegPerSec = 90.0;
        private double SectorHalfAngleDeg = 10.0; // sector total = 20 deg
        private double radarAngleRad = 0.0; // current sweep direction (radians)

        // Visual helpers
        //private TranslateTransform3D threatTrans;
        //private TranslateTransform3D launcherTrans;
        //private TranslateTransform3D explosionTrans;
        double _ExplosionSphereOffsetX = 0;
        public double ExplosionSphereOffsetX
        {
            get => _ExplosionSphereOffsetX;
            set
            {
                _ExplosionSphereOffsetX = value;
                OnPropertyChanged();
            }
        }
        double _ExplosionSphereOffsetY = 0;
        public double ExplosionSphereOffsetY
        {
            get => _ExplosionSphereOffsetY;
            set
            {
                _ExplosionSphereOffsetY = value;
                OnPropertyChanged();
            }
        }
        double _ExplosionSphereOffsetZ = 0;
        public double ExplosionSphereOffsetZ
        {
            get => _ExplosionSphereOffsetZ;
            set
            {
                _ExplosionSphereOffsetZ = value;
                OnPropertyChanged();
            }
        }
        //ThreatSphereOffsetX
        double _ThreatSphereOffsetX = 0;
        public double ThreatSphereOffsetX
        {
            get => _ThreatSphereOffsetX;
            set
            {
                _ThreatSphereOffsetX = value;
                OnPropertyChanged();
            }
        }
        double _ThreatSphereOffsetY = 0;
        public double ThreatSphereOffsetY
        {
            get => _ThreatSphereOffsetY;
            set
            {
                _ThreatSphereOffsetY = value;
                OnPropertyChanged();
            }
        }
        double _ThreatSphereOffsetZ = 0;
        public double ThreatSphereOffsetZ
        {
            get => _ThreatSphereOffsetZ;
            set
            {
                _ThreatSphereOffsetZ = value;
                OnPropertyChanged();
            }
        }
        double _LauncherOffsetX = 0;
        public double LauncherOffsetX
        {
            get => _LauncherOffsetX;
            set
            {
                _LauncherOffsetX = value;
                OnPropertyChanged();
            }
        }
        double _LauncherOffsetY = 0;
        public double LauncherOffsetY
        {
            get => _LauncherOffsetY;
            set
            {
                _LauncherOffsetY = value;
                OnPropertyChanged();
            }
        }
        double _LauncherOffsetZ = 0;
        public double LauncherOffsetZ
        {
            get => _LauncherOffsetZ;
            set
            {
                _LauncherOffsetZ = value;
                OnPropertyChanged();
            }
        }
        // For circle drawing
        private int circleSegments = 120;

        string strRadarRange = "18";
        public string StrRadarRange
        {
            get => strRadarRange;
            set
            {
                strRadarRange = value;
                OnPropertyChanged();
            }
        }
        string strSweepSpeed = "90";
        public string StrSweepSpeed
        {
            get => strSweepSpeed;
            set
            {
                strSweepSpeed = value;
                OnPropertyChanged();
            }
        }
        string strSectorAngle = "20";
        public string StrSectorAngle
        {
            get => strSectorAngle;
            set
            {
                strSectorAngle = value;
                OnPropertyChanged();
            }
        }
        string strThreatSpeed = "3.0";
        public string StrThreatSpeed
        {
            get => strThreatSpeed;
            set
            {
                strThreatSpeed = value;
                OnPropertyChanged();
            }
        }
        string strThreatInit = "5,5,5";
        public string StrThreatInit
        {
            get => strThreatInit;
            set
            {
                strThreatInit = value;
                OnPropertyChanged();
            }
        }
        string strThreatDir = "1,1,1";
        public string StrThreatDir
        {
            get => strThreatDir;
            set
            {
                strThreatDir = value;
                OnPropertyChanged();
            }
        }
        string strInterceptorSpeed = "8.0";
        public string StrInterceptorSpeed
        {
            get => strInterceptorSpeed;
            set
            {
                strInterceptorSpeed = value;
                OnPropertyChanged();
            }
        }
        string strLauncherPos = "0,0,0";
        public string StrLauncherPos
        {
            get => strLauncherPos;
            set
            {
                strLauncherPos = value;
                OnPropertyChanged();
            }
        }
        string strInfo = "대기중..";
        public string StrInfo
        {
            get => strInfo;
            set
            {
                strInfo = value;
                OnPropertyChanged();
            }
        }

        private List<Point3D> threatPath = new List<Point3D>();
        Point3DCollection _ThreatPath = new Point3DCollection();
        public Point3DCollection ThreatPath 
        {
            get => _ThreatPath;
            set
            {
                _ThreatPath = value;
                OnPropertyChanged();
            }
        }
        Point3DCollection _InterceptorPath = new Point3DCollection();
        public Point3DCollection InterceptorPath
        {
            get => _InterceptorPath;
            set
            {
                _InterceptorPath = value;
                OnPropertyChanged();
            }
        }
        Point3DCollection _RadarCircle = new Point3DCollection();
        public Point3DCollection RadarCircle
        {
            get => _RadarCircle;
            set
            {
                _RadarCircle = value;
                OnPropertyChanged();
            }
        }

        bool _IsVisibleExplosionSphere = false;
        public bool IsVisibleExplosionSphere 
        {
            get => _IsVisibleExplosionSphere;
            set
            {
                _IsVisibleExplosionSphere = value;
                OnPropertyChanged();
            }
        }

        Point3DCollection _RadarSweepLine = new Point3DCollection();
        public Point3DCollection RadarSweepLine
        {
            get => _RadarSweepLine;
            set
            {
                _RadarSweepLine = value;
                OnPropertyChanged();
            }
        }

        GeometryModel3D _RadarSectorModel;
        public GeometryModel3D RadarSectorModel
        {
            get => _RadarSectorModel;
            set
            {
                _RadarSectorModel = value;
                OnPropertyChanged();
            }
        }

        double _RadarSweepLine2DX1 = 0;
        public double RadarSweepLine2DX1
        {
            get => _RadarSweepLine2DX1;
            set
            {
                _RadarSweepLine2DX1 = value;
                OnPropertyChanged();
            }
        }
        double _RadarSweepLine2DX2 = 0;
        public double RadarSweepLine2DX2
        {
            get => _RadarSweepLine2DX2;
            set
            {
                _RadarSweepLine2DX2 = value;
                OnPropertyChanged();
            }
        }
        double _RadarSweepLine2DY1 = 0;
        public double RadarSweepLine2DY1
        {
            get => _RadarSweepLine2DY1;
            set
            {
                _RadarSweepLine2DY1 = value;
                OnPropertyChanged();
            }
        }
        double _RadarSweepLine2DY2 = 0;
        public double RadarSweepLine2DY2
        {
            get => _RadarSweepLine2DY2;
            set
            {
                _RadarSweepLine2DY2 = value;
                OnPropertyChanged();
            }
        }

        double _DetectedX;
        public double DetectedX
        {
            get => _DetectedX;
            set
            {
                _DetectedX = value;
                OnPropertyChanged();
            }
        }
        double _DetectedY;
        public double DetectedY
        {
            get => _DetectedY;
            set
            {
                _DetectedY = value;
                OnPropertyChanged();
            }
        }
        double _DetectedEllipseOpacity;
        public double DetectedEllipseOpacity
        {
            get => _DetectedEllipseOpacity;
            set
            {
                _DetectedEllipseOpacity = value;
                OnPropertyChanged();
            }
        }
        Visibility _DetectedEllipseVisibility;
        public Visibility DetectedEllipseVisibility
        {
            get => _DetectedEllipseVisibility;
            set
            {
                _DetectedEllipseVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public void StartMission()
        {
            if (!ReadParameters()) return;
            DrawRadarCircle();
            interceptorLaunched = false;
            interceptorModel = null;
            interceptorPath.Clear();
            InterceptorPath = new Point3DCollection(interceptorPath);
            IsVisibleExplosionSphere = false;//Visibility.Collapsed;
            timer.Start();
            StrInfo = "레이더 동작 중...";
        }

        public void StopMission()
        {
            timer.Stop();
            StrInfo = "중지됨.";
        }

        public void ResetMission()
        {
            timer.Stop();
            ResetSimulation();
        }



        #endregion

        #region Initialization & Parameter Parsing
        private void ResetSimulation()
        {
            // read UI or use defaults
            threatSpeed = double.TryParse(StrThreatSpeed, out var ts) ? ts : 3.0;
            interceptorSpeed = double.TryParse(StrInterceptorSpeed, out var ispd) ? ispd : 8.0;
            RadarRange = double.TryParse(StrRadarRange, out var rr) ? rr : 18.0;
            SweepAngleDegPerSec = double.TryParse(StrSweepSpeed, out var ss) ? ss : 90.0;
            SectorHalfAngleDeg = double.TryParse(StrSectorAngle, out var sac) ? sac / 2.0 : 10.0;

            threatPos = ParsePoint3D(StrThreatInit);
            var tdir = ParseVector3D(StrThreatDir);
            if (tdir.Length == 0) tdir = new Vector3D(-1, 0, 0);
            tdir.Normalize();
            threatVel = tdir * threatSpeed;

            radarPos = ParsePoint3D(StrLauncherPos);

            // place transforms

            ThreatSphereOffsetX = threatPos.X;
            ThreatSphereOffsetY = threatPos.Y;
            ThreatSphereOffsetZ = threatPos.Z;

            ExplosionSphereOffsetX = radarPos.X;
            ExplosionSphereOffsetY = radarPos.Y;
            ExplosionSphereOffsetZ = radarPos.Z;

            // clear paths
            threatPath = new List<Point3D>() { threatPos };
            ThreatPath = new Point3DCollection(threatPath);
            interceptorPath.Clear();
            InterceptorPath = new Point3DCollection(interceptorPath);

            // radar visuals
            DrawRadarCircle();
            DrawRadarSectorVisual(); // initial sector mesh
            RadarSweepLine = new Point3DCollection();

            // reset missile
            if (interceptorModel != null)
            {
                //view3d.Children.Remove(interceptorModel);
                interceptorModel = null;
            }
            interceptorLaunched = false;
            IsVisibleExplosionSphere = false;// Visibility.Collapsed;

            StrInfo = "리셋 완료. Start를 눌러 실행하세요.";
        }

        private bool ReadParameters()
        {
            try
            {
                // re-read params from UI
                threatSpeed = double.Parse(StrThreatSpeed, CultureInfo.InvariantCulture);
                interceptorSpeed = double.Parse(StrInterceptorSpeed, CultureInfo.InvariantCulture);
                RadarRange = double.Parse(StrRadarRange, CultureInfo.InvariantCulture);
                SweepAngleDegPerSec = double.Parse(StrSweepSpeed, CultureInfo.InvariantCulture);
                double sector = double.Parse(StrSectorAngle, CultureInfo.InvariantCulture);
                SectorHalfAngleDeg = sector / 2.0;

                threatPos = ParsePoint3D(StrThreatInit);
                var tdir = ParseVector3D(StrThreatDir);
                if (tdir.Length == 0) tdir = new Vector3D(-1, 0, 0);
                tdir.Normalize();
                threatVel = tdir * threatSpeed;

                radarPos = ParsePoint3D(StrLauncherPos);

                // apply
                ThreatSphereOffsetX = threatPos.X;
                ThreatSphereOffsetY = threatPos.Y;
                ThreatSphereOffsetZ = threatPos.Z;

                LauncherOffsetX = radarPos.X; 
                LauncherOffsetY = radarPos.Y; 
                LauncherOffsetZ = radarPos.Z;

                // clear path lists
                threatPath = new List<Point3D>() { threatPos };
                ThreatPath = new Point3DCollection(threatPath);
                interceptorPath.Clear();
                InterceptorPath = new Point3DCollection(interceptorPath);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("파라미터 읽기 실패: " + ex.Message);
                return false;
            }
        }
        #endregion

        #region Radar Visuals
        private double NormalizeAngle(double a)
        {
            while (a > Math.PI) a -= 2 * Math.PI;
            while (a < -Math.PI) a += 2 * Math.PI;
            return a;
        }
        private void DrawRadarCircle()
        {
            var pts = new Point3DCollection();
            for (int i = 0; i <= circleSegments; i++)
            {
                double ang = 2.0 * Math.PI * i / circleSegments;
                double x = radarPos.X + RadarRange * Math.Cos(ang);
                double y = radarPos.Z + RadarRange * Math.Sin(ang);
                double z = radarPos.Y;
                pts.Add(new Point3D(x, y, z));
            }
            RadarCircle = pts;
        }

        private void DrawRadarSweepLine()
        {
            double ang = radarAngleRad;
            double x2 = radarPos.X + RadarRange * Math.Cos(ang);
            double y2 = radarPos.Y + RadarRange * Math.Sin(ang);
            var pts = new Point3DCollection(new[] {
                new Point3D(radarPos.X, radarPos.Y, radarPos.Z),
                //new Point3D(x2, radarPos.Y, z2)
                new Point3D(x2, y2, radarPos.Z)
            });
            RadarSweepLine = pts;
        }

        private void DrawRadarSectorVisual()
        {
            // Sector mesh centered at radarPos, in XZ plane, spanning [-half, +half] around radarAngleRad
            var mb = new MeshBuilder(false, false);
            int seg = 36;
            double half = SectorHalfAngleDeg * Math.PI / 180.0;
            double start = radarAngleRad - half;
            double end = radarAngleRad + half;
            var geom = mb.ToMesh();
            var mat = MaterialHelper.CreateMaterial(new SolidColorBrush(Color.FromArgb(100, 50, 205, 50))); // semi-transparent green
            var gm = new GeometryModel3D { Geometry = geom, Material = mat, BackMaterial = mat };

            RadarSectorModel = gm;
        }
        #endregion

        #region Missile Model (procedural) - missile geometry
        private ModelVisual3D CreateMissileModel()
        {
            var mb = new MeshBuilder(true, true);

            double bodyLength = 1.2;
            double bodyRadius = 0.12;
            double noseLength = 0.35;

            // We'll align missile along +X
            var p0 = new Point3D(0, 0, 0);
            var p1 = new Point3D(bodyLength, 0, 0);
            mb.AddCylinder(p0, p1, bodyRadius * 2, 24);

            var noseTip = new Point3D(bodyLength + noseLength, 0, 0);
            mb.AddCone(p1, noseTip, bodyRadius * 2, true, 24);

            var mesh = mb.ToMesh(true);
            var mat = MaterialHelper.CreateMaterial(Brushes.DarkGray);
            var gm = new GeometryModel3D { Geometry = mesh, Material = mat, BackMaterial = mat };
            return new ModelVisual3D { Content = gm };
        }
        #endregion

        #region Main simulation tick
        private void Timer_Tick(object sender, EventArgs e)
        {
            // 1) Update radar sweep angle
            double sweepRadPerSec = SweepAngleDegPerSec * Math.PI / 180.0;
            radarAngleRad += sweepRadPerSec * dt;
            // normalize
            radarAngleRad = (radarAngleRad + Math.PI * 2) % (Math.PI * 2);

            // update sweep visuals
            DrawRadarSweepLine();
            DrawRadarSectorVisual();

            // 2) Move threat (linear motion)
            threatPos = new Point3D(threatPos.X + threatVel.X * dt,
                                    threatPos.Y + threatVel.Y * dt,
                                    threatPos.Z + threatVel.Z * dt);
            ThreatSphereOffsetX = threatPos.X; 
            ThreatSphereOffsetY = threatPos.Y; 
            ThreatSphereOffsetZ = threatPos.Z;
            threatPath.Add(threatPos);
            ThreatPath = new Point3DCollection(threatPath);

            // 3) Radar detection check (if not already launched)
            if (!interceptorLaunched)
            {
                if (IsThreatDetectedByRadar())
                {
                    interceptorLaunched = true;
                    LaunchInterceptor();
                    StrInfo = "탐지: Threat 발견! 유도탄 발사.";
                }
            }

            // 4) If interceptor launched, run intercept logic (lead intercept)
            if (interceptorLaunched && interceptorModel != null)
            {
                LauncherOffsetX = interceptorPos.X;
                LauncherOffsetY = interceptorPos.Y;
                LauncherOffsetZ = interceptorPos.Z;
                // intercept velocity computed via lead intercept quadratic (same as earlier)
                // compute relative vectors
                Vector3D r = (Vector3D)(threatPos - interceptorPos);
                Vector3D v = threatVel;
                double s = interceptorSpeed;

                double a = 0;// v.DotProduct(v) - s * s;
                double b = 2 * Vector3D.DotProduct(v, r);
                double c = Vector3D.DotProduct(r, r);

                double tIntercept = double.NaN;
                if (Math.Abs(a) < 1e-6)
                {
                    if (Math.Abs(b) > 1e-6)
                    {
                        double t = -c / b;
                        if (t > 0) tIntercept = t;
                    }
                }
                else
                {
                    double disc = b * b - 4 * a * c;
                    if (disc >= 0)
                    {
                        double sqrtD = Math.Sqrt(disc);
                        double t1 = (-b + sqrtD) / (2 * a);
                        double t2 = (-b - sqrtD) / (2 * a);
                        var cand = new[] { t1, t2 }.Where(tt => tt > 1e-6).ToArray();
                        if (cand.Length > 0) tIntercept = cand.Min();
                    }
                }

                Vector3D desiredDir;
                if (!double.IsNaN(tIntercept))
                {
                    var aim = new Point3D(threatPos.X + threatVel.X * tIntercept,
                                          threatPos.Y + threatVel.Y * tIntercept,
                                          threatPos.Z + threatVel.Z * tIntercept);
                    desiredDir = (Vector3D)(aim - interceptorPos);
                    if (desiredDir.Length > 1e-6) desiredDir.Normalize();
                }
                else
                {
                    desiredDir = (Vector3D)(threatPos - interceptorPos);
                    if (desiredDir.Length > 1e-6) desiredDir.Normalize();
                }

                interceptorVel = desiredDir * interceptorSpeed;

                // integrate interceptor position
                interceptorPos = new Point3D(interceptorPos.X + interceptorVel.X * dt,
                                             interceptorPos.Y + interceptorVel.Y * dt,
                                             interceptorPos.Z + interceptorVel.Z * dt);

                // orientation: make missile point along interceptorVel
                UpdateMissileTransform(interceptorModel, interceptorPos, interceptorVel);

                interceptorPath.Add(interceptorPos);
                InterceptorPath = new Point3DCollection(interceptorPath);

                // check intercept distance
                double dist = Distance(threatPos, interceptorPos);
                StrInfo = $"거리: {dist:F2} | 레이더 각도(deg): {radarAngleRad * 180.0 / Math.PI:F1}";
                if (dist < 0.3)
                {
                    // explosion
                    IsVisibleExplosionSphere = true;
                    ExplosionSphereOffsetX = (threatPos.X + interceptorPos.X) / 2.0;
                    ExplosionSphereOffsetY = (threatPos.Y + interceptorPos.Y) / 2.0;
                    ExplosionSphereOffsetZ = (threatPos.Z + interceptorPos.Z) / 2.0;

                    timer.Stop();
                    StrInfo = $"요격 성공! 충돌 거리 {dist:F2}";
                }
            }

            // 5) safety: out of bounds
            if (Math.Abs(threatPos.X) > 500 || Math.Abs(threatPos.Z) > 500)
            {
                timer.Stop();
                StrInfo = "Threat가 범위를 벗어났습니다. 리셋하세요.";
            }

            // 2D Radar update
            UpdateRadar2DDisplay();
        }
        double age = 0;
        DateTime _nowTime;
        private void UpdateRadar2DDisplay()
        {
            double width = 280;
            double height = 280;
            double centerX = width / 2;
            double centerY = height / 2;
            double scale = (width / 2) / RadarRange;

            // === 1) 스윕선 갱신 ===
            double ang = radarAngleRad;
            double x2 = centerX + (RadarRange * Math.Cos(ang)) * scale;
            double y2 = centerY - (RadarRange * Math.Sin(ang)) * scale;

            RadarSweepLine2DX1= centerX;
            RadarSweepLine2DY1 = centerY;
            RadarSweepLine2DX2 = x2;
            RadarSweepLine2DY2 = y2;

            // === 2) 기존 blip 잔상 업데이트 ===
            double fadeDuration = 4.0; // 초 단위로 잔상 유지시간

            //DetectedEllipseOpacity
            //DetectedEllipseVisibility

            if (DetectedEllipseVisibility == Visibility.Visible) 
            {
                double age = (DateTime.Now - _nowTime).TotalSeconds;
                if (age > fadeDuration)
                {
                    DetectedEllipseVisibility = Visibility.Collapsed;
                }
                else
                {
                    // 남은 비율에 따라 투명도 감소
                    DetectedEllipseOpacity = Math.Max(0, 1.0 - age / fadeDuration);
                }
            }

            // === 3) Threat이 스윕에 닿았는지 검사 ===
            double dx = threatPos.X - radarPos.X;
            double dy = threatPos.Y - radarPos.Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);

            if (dist <= RadarRange)
            {
                double threatAng = Math.Atan2(dy, dx);
                double delta = Math.Abs(NormalizeAngle(threatAng - radarAngleRad));

                double scanWidthDeg = 2.0; // ±2도 이내 감지
                if (delta < scanWidthDeg * Math.PI / 180.0)
                {
                    double tx = centerX + dx * scale;
                    double ty = centerY - dy * scale;

                    _nowTime = DateTime.Now;

                    DetectedX = tx - 4;
                    DetectedY = ty - 4;
                    DetectedEllipseVisibility = Visibility.Visible;
                }
            }
        }

        #endregion

        #region Radar detection math
        private bool IsThreatDetectedByRadar()
        {
            // range test
            double d = Distance(threatPos, radarPos);
            if (d > RadarRange) return false;

            // direction test: project onto XZ plane (radar scans horizontally)
            Vector3D toTarget = (Vector3D)(threatPos - radarPos);
            // ignore vertical for angular test, but we still require some vertical window optionally
            Vector3D toTargetXY = new Vector3D(toTarget.X, toTarget.Y, 0);
            if (toTargetXY.Length < 1e-6) return true; // on top

            toTargetXY.Normalize();
            Vector3D radarDir = new Vector3D(Math.Cos(radarAngleRad), Math.Sin(radarAngleRad), 0);
            double dot = Vector3D.DotProduct(radarDir, toTargetXY);
            double ang = Math.Acos(Math.Max(-1.0, Math.Min(1.0, dot))); // radians

            return ang <= (SectorHalfAngleDeg * Math.PI / 180.0);
        }
        #endregion

        #region Launch interceptor
        private void LaunchInterceptor()
        {
            // create missile model
            interceptorModel = CreateMissileModel();
            //view3d.Children.Add(interceptorModel);

            // initialize interceptor position and velocity
            interceptorPos = ParsePoint3D(StrLauncherPos);
            interceptorVel = new Vector3D(0, 0, 0);
            UpdateMissileTransform(interceptorModel, interceptorPos, interceptorVel);

            interceptorPath = new List<Point3D>() { interceptorPos };
            InterceptorPath = new Point3DCollection(interceptorPath);
        }
        #endregion

        #region Missile transform update (rotate to velocity + translate)
        private void UpdateMissileTransform(ModelVisual3D missile, Point3D pos, Vector3D vel)
        {
            // forward vector of model is +X
            Vector3D forward = new Vector3D(1, 0, 0);
            Vector3D dir = vel;
            if (dir.LengthSquared < 1e-8)
            {
                // no velocity — keep default orientation
                var tgId = new Transform3DGroup();
                tgId.Children.Add(new TranslateTransform3D(pos.X, pos.Y, pos.Z));
                missile.Transform = tgId;
                return;
            }
            dir.Normalize();

            Vector3D axis = Vector3D.CrossProduct(forward, dir);
            double dot = Vector3D.DotProduct(forward, dir);
            double angle = Math.Acos(Math.Max(-1.0, Math.Min(1.0, dot))); // radians

            Quaternion q;
            if (axis.LengthSquared < 1e-8)
            {
                if (dot < 0) q = new Quaternion(new Vector3D(0, 1, 0), 180);
                else q = Quaternion.Identity;
            }
            else
            {
                axis.Normalize();
                q = new Quaternion(axis, angle * 180.0 / Math.PI);
            }

            var rot = new RotateTransform3D(new QuaternionRotation3D(q));
            var trans = new TranslateTransform3D(pos.X, pos.Y, pos.Z);
            var tg = new Transform3DGroup();
            tg.Children.Add(rot);
            tg.Children.Add(trans);
            missile.Transform = tg;
        }
        #endregion

        #region Utilities
        private Point3D ParsePoint3D(string s)
        {
            var parts = s.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            double x = 0, y = 0, z = 0;
            if (parts.Length >= 1) double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out x);
            if (parts.Length >= 2) double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out y);
            if (parts.Length >= 3) double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out z);
            return new Point3D(x, y, z);
        }
        private Vector3D ParseVector3D(string s)
        {
            var parts = s.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            double x = 0, y = 0, z = 0;
            if (parts.Length >= 1) double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out x);
            if (parts.Length >= 2) double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out y);
            if (parts.Length >= 3) double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out z);
            return new Vector3D(x, y, z);
        }
        private double Distance(Point3D a, Point3D b)
        {
            var d = a - b;
            return Math.Sqrt(d.X * d.X + d.Y * d.Y + d.Z * d.Z);
        }
        #endregion

        #region Property Update
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}