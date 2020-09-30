using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleLed;

namespace Source.gridripple
{
    public class GridRipple : ISimpleLed
    {
        public event EventHandler DeviceRescanRequired;
        public ControlDevice.LedUnit[] leds = new ControlDevice.LedUnit[60*12];

        public void Dispose()
        {
            
        }
        
        public void Configure(DriverDetails driverDetails)
        {
            int ct = 0;
            for (int y = 0; y < 12; y++)
            {
                for (int x = 0; x < 60; x++)
                {
                    leds[ct] = new ControlDevice.LedUnit
                    {
                        Color = new LEDColor(0,0,0),
                        LEDName = x+","+y,
                        Data = new ControlDevice.PositionalLEDData
                        {
                            LEDNumber = ct,
                            X = x,
                            Y=y
                        }
                    };
                    ct++;
                }
            }
        }

        private ControlDevice myControlDevice;
        public List<ControlDevice> GetDevices()
        {
            myControlDevice = new ControlDevice
            {
                DeviceType = DeviceTypes.Effect,
                Driver = this,
                GridHeight = 12,
                GridWidth = 60,
                Has2DSupport = true,
                LEDs = leds,
                Name = "Grid Ripple",

            };

            myControlDevice.DestTriggeredEvent += MyControlDeviceOnDestTriggeredEvent;

            return new List<ControlDevice>
            {
                myControlDevice
            };
        }

        private void MyControlDeviceOnDestTriggeredEvent(object sender, ControlDevice.TriggerEventArgs e)
        {
            Particles.Add(new Particle
            {
                X = (int)(e.FloatX * myControlDevice.GridWidth),
                Y = (int)(e.FloatY * myControlDevice.GridHeight),
                Distance = 0,
                Strength = 1,
                R=255,G=0,B=255
            });

            Particles.Add(new Particle
            {
                X = (int)(e.FloatX * myControlDevice.GridWidth),
                Y = (int)(e.FloatY * myControlDevice.GridHeight),
                Distance = 1,
                Strength = 1,
                R=0,G=0,B=0
            });
        }
        
        public void Push(ControlDevice controlDevice)
        {
        }

        public void Pull(ControlDevice controlDevice)
        {
            foreach (var ledUnit in leds)
            {
                if (ledUnit.Color == null)
                {
                    ledUnit.Color=new LEDColor(255,0,0);
                }

                ledUnit.Color = ledUnit.Color.LerpTo(new LEDColor(255, 0, 0), 0.01f);

            }

            foreach (Particle particle in Particles.ToList())
            {
                int top = particle.Y - particle.Distance;
                int left = particle.X - particle.Distance;

                int bottom = particle.Y + particle.Distance;
                int right = particle.X + particle.Distance;

                for (int x = left; x < right; x++)
                {
                    for (int y = top; y < bottom; y++)
                    {
                        if (x == left || x + 1 == right || y == top || y + 1 == bottom)
                        {
                            myControlDevice.SetGridLED(x, y, new LEDColor((int)((float)particle.R * particle.Strength), (int)((float)particle.G * particle.Strength), (int) ((float)particle.B * particle.Strength)));
                        }
                    }
                }

                particle.Distance++;
                particle.Strength = particle.Strength * 0.95f;
            }

            List<Particle> deadParticles = Particles.ToList().Where(x => x.Strength < 0.05f).ToList();

            foreach (var deadParticle in deadParticles)
            {
                Particles.Remove(deadParticle);
            }
        }

        public class Particle
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Distance { get; set; } = 1;
            public float Strength { get; set; } = 1f;

            public int R { get; set; }
            public int G { get; set; }
            public int B { get; set; }

        }

        public List<Particle> Particles { get; set; } = new List<Particle>();

        public DriverProperties GetProperties()
        {
            return new DriverProperties
            {
                Author = "mad ninja",
                Blurb = "Example effect with 2d support and basic interactivity",
                GitHubLink = "https://github.com/SimpleLed/Source.gridripple",
                CurrentVersion = new ReleaseNumber(0,0,0,1002),
                Id = Guid.Parse("78c8f449-0148-476b-a6de-b490a9f92017"),
                IsPublicRelease = false,
                IsSource = true,
                SupportsPull = true
            };
        }

        public T GetConfig<T>() where T : SLSConfigData
        {
            return null;
        }

        public void PutConfig<T>(T config) where T : SLSConfigData
        {
         
        }

        public string Name()
        {
            return "Grid Ripple";
        }
    }
}
