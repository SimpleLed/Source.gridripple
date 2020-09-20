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
                Distance = 1,
                Strength = 1
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

                ledUnit.Color = ledUnit.Color.LerpTo(new LEDColor(255, 0, 0), 0.1f);

                //if (ledUnit.Color.Red < 255)
                //{
                //    ledUnit.Color.Red = ledUnit.Color.Red +1;
                //}

                //if (ledUnit.Color.Green > 0)
                //{
                //    ledUnit.Color.Green = ledUnit.Color.Green / 2;
                //}


                //if (ledUnit.Color.Blue > 0)
                //{
                //    ledUnit.Color.Blue = ledUnit.Color.Blue / 2;
                //}
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
                        myControlDevice.SetGridLED(x,y,new LEDColor(0, 0, (int)(255f * particle.Strength)));
                    }
                }

                particle.Distance++;
                particle.Strength = particle.Strength * 0.7f;
            }

            List<Particle> deadParticles = Particles.Where(x => x.Strength < 0.05f).ToList();

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
        }

        public List<Particle> Particles { get; set; } = new List<Particle>();

        public DriverProperties GetProperties()
        {
            return new DriverProperties
            {
                Author = "mad ninja",
                Blurb = "Example effect with 2d support and basic interactivity",
                GitHubLink = "https://github.com/SimpleLed/Source.gridripple",
                CurrentVersion = new ReleaseNumber(0,0,0,1001),
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
