using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using System;
using System.Windows.Input;

namespace HeavenUsurperToolkit
{
    public class Particle
    {
        public Ellipse Shape { get; set; }
        public double Angle { get; set; }
        public double Speed { get; set; }
        public double Age { get; set; }
        public double LifeSpan { get; set; }
    }

    public class ParticleEffectManager
    {
        public Canvas ParticleCanvas { get; private set; }
        private readonly List<Particle> _particles = new List<Particle>();
        private readonly Random _random = new Random();

        private double _particleSpeed = 1.2;
        private const double DefaultSpeed = 0.15;
        private const double MaxSpeed = 3;
        private const double SpeedDecay = 0.1;

        private int _particleCount = 1; // 默认粒子数量
        private const int DefaultParticleCount = 1;
        private const int MaxParticleCount = 10;
        private const double ParticleCountDecay = 0.5;

        private Point _mousePosition;

        public bool IsParticleEffectEnabled { get; set; }

        public ParticleEffectManager(Canvas particleCanvas)
        {
            ParticleCanvas = particleCanvas;
            IsParticleEffectEnabled = false;
        }

        public void SetMousePosition(Point position)
        {
            _mousePosition = position;
        }

        public void TriggerParticleSpeedBoost()
        {
            if (IsParticleEffectEnabled)
            {
                _particleSpeed = MaxSpeed;
            }
        }

        public void TriggerParticleCountBoost()
        {
            if (IsParticleEffectEnabled)
            {
                _particleCount = MaxParticleCount;
            }
        }

        public void UpdateParticles()
        {
            if (IsParticleEffectEnabled)
            {
                // 粒子速度衰减
                if (_particleSpeed > DefaultSpeed)
                {
                    _particleSpeed -= SpeedDecay;
                    if (_particleSpeed < DefaultSpeed)
                    {
                        _particleSpeed = DefaultSpeed;
                    }
                }

                // 粒子数量衰减
                if (_particleCount > DefaultParticleCount)
                {
                    _particleCount = (int)Math.Max(DefaultParticleCount, _particleCount - ParticleCountDecay);
                }

                // 创建新粒子
                for (int i = 0; i < _particleCount; i++)
                {
                    CreateParticle(_mousePosition);
                }

                // 更新和移除粒子
                for (int i = _particles.Count - 1; i >= 0; i--)
                {
                    Particle particle = _particles[i];
                    particle.Age += 1;

                    if (particle.Age > particle.LifeSpan)
                    {
                        ParticleCanvas.Children.Remove(particle.Shape);
                        _particles.RemoveAt(i);
                    }
                    else
                    {
                        // 结合粒子自身速度和全局粒子速度
                        double currentSpeed = particle.Speed * _particleSpeed;

                        double x = Canvas.GetLeft(particle.Shape) + Math.Cos(particle.Angle) * currentSpeed;
                        double y = Canvas.GetTop(particle.Shape) + Math.Sin(particle.Angle) * currentSpeed;

                        Canvas.SetLeft(particle.Shape, x);
                        Canvas.SetTop(particle.Shape, y);

                        // 逐渐改变粒子颜色和大小
                        double progress = particle.Age / particle.LifeSpan;
                        byte alpha = (byte)(255 * (1 - progress));
                        particle.Shape.Fill = new SolidColorBrush(Color.FromArgb(alpha, 255, 255, 255));
                        double size = 5 * (1 - progress);
                        particle.Shape.Width = size;
                        particle.Shape.Height = size;
                    }
                }
            } else {
                // 如果粒子效果未启用，清除所有现有粒子
                foreach (var particle in _particles)
                {
                    ParticleCanvas.Children.Remove(particle.Shape);
                }
                _particles.Clear();
            }
        }

        private void CreateParticle(Point position)
        {
            Ellipse particleShape = new Ellipse
            {
                Width = 5,
                Height = 5,
                Fill = new SolidColorBrush(Colors.White)
            };

            ParticleCanvas.Children.Add(particleShape);
            Canvas.SetLeft(particleShape, position.X - particleShape.Width / 2);
            Canvas.SetTop(particleShape, position.Y - particleShape.Height / 2);

            _particles.Add(new Particle
            {
                Shape = particleShape,
                Angle = _random.NextDouble() * 2 * Math.PI,
                Speed = _random.NextDouble() * 2 + 1, // 粒子自身速度
                Age = 0,
                LifeSpan = _random.Next(30, 90)
            });
        }
    }
}