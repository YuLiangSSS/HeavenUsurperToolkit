using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeavenUsurperToolkit
{
    public interface IResizablePage
    {
        double DesiredWidth { get; }
        double DesiredHeight { get; }
    }



    public partial class MainWindow : Window
    {
        private Page? _currentPage;
        private Page? _newPage;
        // 添加一个标志变量来跟踪动画是否正在进行
        private bool _isNavigating = false;

        private ParticleEffectManager _particleEffectManager;



        private bool _particleEffectActivatedByButton = false;

        public MainWindow()
        {
            InitializeComponent();

            // 默认导航到第一个页面
            NavigateToPage(new MainPage());

            // 初始化 ParticleEffectManager，传入 ParticleCanvas
            _particleEffectManager = new ParticleEffectManager(ParticleCanvas);

            this.StateChanged += Window_StateChanged;
            this.MouseMove += MainWindow_MouseMove;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            PreviewMouseLeftButtonDown += MainWindow_PreviewMouseLeftButtonDown;
            PreviewMouseRightButtonDown += MainWindow_PreviewMouseRightButtonDown;
            MouseEnter += MainWindow_MouseEnter;
            MouseLeave += MainWindow_MouseLeave;
        }

        // 将方法改为公共方法，以便其他页面可以调用
        public void NavigateToPage(Page newPage)
        {
            // 如果正在导航中，则忽略此次导航请求
            if (_isNavigating)
                return;

            // 如果当前没有页面或者要导航的页面与当前页面类型相同，则不执行动画
            if (_currentPage == null || _currentPage.GetType() == newPage.GetType())
            {
                MainFrame.Content = newPage;
                _currentPage = newPage;
                // 立即调整窗口大小以适应新页面
                AdjustWindowSize(newPage);
                return;
            }

            // 设置导航标志为正在导航
            _isNavigating = true;
            _newPage = newPage;

            // 创建一个新的Frame来承载新页面
            Frame newFrame = new Frame();
            newFrame.Content = newPage;

            // 设置新Frame的初始位置（在屏幕右侧）
            newFrame.Margin = new Thickness(MainFrame.ActualWidth, 0, -MainFrame.ActualWidth, 0);

            // 将新Frame添加到ContentArea
            ContentArea.Children.Add(newFrame);

            // 创建easeOutExpo缓动函数
            ExponentialEase easeOutExpo = new ExponentialEase();
            easeOutExpo.EasingMode = EasingMode.EaseOut;
            easeOutExpo.Exponent = 6; // 指数值，越大效果越明显

            ExponentialEase easeInExpo = new ExponentialEase();
            easeInExpo.EasingMode = EasingMode.EaseIn;
            easeInExpo.Exponent = 6;

            ExponentialEase easeInOutExpo = new ExponentialEase();
            easeInOutExpo.EasingMode= EasingMode.EaseInOut;
            easeInOutExpo.Exponent = 6;

            // 创建当前页面滑出动画
            var slideOutAnimation = new ThicknessAnimation
            {
                Duration = TimeSpan.FromSeconds(0.5),
                From = new Thickness(0, 0, 0, 0),
                To = new Thickness(-MainFrame.ActualWidth, 0, MainFrame.ActualWidth, 0),
                EasingFunction = easeInOutExpo
            };
            Storyboard.SetTarget(slideOutAnimation, MainFrame);
            Storyboard.SetTargetProperty(slideOutAnimation, new PropertyPath(FrameworkElement.MarginProperty));

            // 创建新页面滑入动画
            var slideInAnimation = new ThicknessAnimation
            {
                Duration = TimeSpan.FromSeconds(0.5),
                From = new Thickness(MainFrame.ActualWidth, 0, -MainFrame.ActualWidth, 0),
                To = new Thickness(0, 0, 0, 0),
                EasingFunction = easeInOutExpo
            };
            Storyboard.SetTarget(slideInAnimation, newFrame);
            Storyboard.SetTargetProperty(slideInAnimation, new PropertyPath(FrameworkElement.MarginProperty));

            var storyboard = new Storyboard();

            // 当动画完成后，清理旧页面
            slideOutAnimation.Completed += (s, e) =>
            {
                // 移除旧的Frame
                ContentArea.Children.Remove(MainFrame);

                // 将新Frame设为主Frame
                MainFrame = newFrame;
                _currentPage = _newPage;

                // 重置导航标志
                _isNavigating = false;
            };

            storyboard.Children.Add(slideOutAnimation);
            storyboard.Children.Add(slideInAnimation);

            // 检查新页面是否实现了IResizablePage接口
            if (newPage is IResizablePage resizablePage)
            {
                // 获取新页面的期望大小
                double newWidth = resizablePage.DesiredWidth;
                double newHeight = resizablePage.DesiredHeight;

                // 获取当前窗口的实际大小
                double currentWidth = this.ActualWidth;
                double currentHeight = this.ActualHeight;

                // 创建宽度动画
                DoubleAnimation widthAnimation = new DoubleAnimation();
                widthAnimation.From = currentWidth;
                widthAnimation.To = newWidth;
                widthAnimation.Duration = TimeSpan.FromSeconds(0.60);
                widthAnimation.EasingFunction = easeInOutExpo;

                // 创建高度动画
                DoubleAnimation heightAnimation = new DoubleAnimation();
                heightAnimation.From = currentHeight;
                heightAnimation.To = newHeight;
                heightAnimation.Duration = TimeSpan.FromSeconds(0.60);
                heightAnimation.EasingFunction = easeInOutExpo;

                // 将动画添加到Storyboard
                Storyboard.SetTarget(widthAnimation, this);
                Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(Window.WidthProperty));

                Storyboard.SetTarget(heightAnimation, this);
                Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(Window.HeightProperty));

                storyboard.Children.Add(widthAnimation);
                storyboard.Children.Add(heightAnimation);
            }

            storyboard.Begin();
            // 调整窗口大小以适应新页面
            AdjustWindowSize(newPage);
        }

        private void AdjustWindowSize(Page page)
        {
            if (page is IResizablePage resizablePage)
            {
                this.Width = resizablePage.DesiredWidth;
                this.Height = resizablePage.DesiredHeight;
            }
        }

        public void ToggleParticleEffect()
        {
            _particleEffectActivatedByButton = !_particleEffectActivatedByButton;
            _particleEffectManager.IsParticleEffectEnabled = _particleEffectActivatedByButton;
        }

        private void MainWindow_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_particleEffectActivatedByButton)
            {
                _particleEffectManager.IsParticleEffectEnabled = true;
            }
        }

        private void MainWindow_MouseLeave(object sender, MouseEventArgs e)
        {
            _particleEffectManager.IsParticleEffectEnabled = false;
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (_particleEffectManager.IsParticleEffectEnabled)
            {
                _particleEffectManager.SetMousePosition(e.GetPosition(this));
            }
        }

        private void MainWindow_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 获取鼠标在窗口中的位置
            Point mousePosition = e.GetPosition(this);

            if (_particleEffectManager.IsParticleEffectEnabled)
            {
                _particleEffectManager.SetMousePosition(mousePosition);
                _particleEffectManager.TriggerParticleSpeedBoost();
                _particleEffectManager.TriggerParticleCountBoost();
            }
        }

        private void MainWindow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_particleEffectManager.IsParticleEffectEnabled)
            {
                _particleEffectManager.TriggerParticleCountBoost();
            }
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            _particleEffectManager.UpdateParticles();
        }

        // 添加一个新方法，用于实现反向动画的导航
        public void NavigateBack(Page newPage)
        {
            // 如果正在导航中，则忽略此次导航请求
            if (_isNavigating)
                return;

            // 如果当前没有页面或者要导航的页面与当前页面类型相同，则不执行动画
            if (_currentPage == null || _currentPage.GetType() == newPage.GetType())
            {
                MainFrame.Content = newPage;
                _currentPage = newPage;
                // 立即调整窗口大小以适应新页面
                AdjustWindowSize(newPage);
                return;
            }

            // 设置导航标志为正在导航
            _isNavigating = true;
            _newPage = newPage;

            // 创建一个新的Frame来承载新页面
            Frame newFrame = new Frame();
            newFrame.Content = newPage;

            // 设置新Frame的初始位置（在屏幕左侧）
            newFrame.Margin = new Thickness(-MainFrame.ActualWidth, 0, MainFrame.ActualWidth, 0);

            // 将新Frame添加到ContentArea
            ContentArea.Children.Add(newFrame);

            // 创建easeOutExpo缓动函数
            ExponentialEase easeOutExpo = new ExponentialEase();
            easeOutExpo.EasingMode = EasingMode.EaseOut;
            easeOutExpo.Exponent = 6; // 指数值，越大效果越明显

            // 创建当前页面滑出动画（向右）
            var slideOutAnimation = new ThicknessAnimation
            {
                Duration = TimeSpan.FromSeconds(0.5),
                From = new Thickness(0, 0, 0, 0),
                To = new Thickness(MainFrame.ActualWidth, 0, -MainFrame.ActualWidth, 0),
                EasingFunction = easeOutExpo
            };
            Storyboard.SetTarget(slideOutAnimation, MainFrame);
            Storyboard.SetTargetProperty(slideOutAnimation, new PropertyPath(FrameworkElement.MarginProperty));

            // 创建新页面滑入动画（从左）
            var slideInAnimation = new ThicknessAnimation
            {
                Duration = TimeSpan.FromSeconds(0.5),
                From = new Thickness(-MainFrame.ActualWidth, 0, MainFrame.ActualWidth, 0),
                To = new Thickness(0, 0, 0, 0),
                EasingFunction = easeOutExpo
            };
            Storyboard.SetTarget(slideInAnimation, newFrame);
            Storyboard.SetTargetProperty(slideInAnimation, new PropertyPath(FrameworkElement.MarginProperty));

            var storyboard = new Storyboard();

            // 当动画完成后，清理旧页面
            slideOutAnimation.Completed += (s, e) =>
            {
                // 移除旧的Frame
                ContentArea.Children.Remove(MainFrame);

                // 将新Frame设为主Frame
                MainFrame = newFrame;
                _currentPage = _newPage;

                // 重置导航标志
                _isNavigating = false;
            };

            storyboard.Children.Add(slideOutAnimation);
            storyboard.Children.Add(slideInAnimation);

            // 检查新页面是否实现了IResizablePage接口
            if (newPage is IResizablePage resizablePage)
            {
                // 获取新页面的期望大小
                double newWidth = resizablePage.DesiredWidth;
                double newHeight = resizablePage.DesiredHeight;

                // 获取当前窗口的实际大小
                double currentWidth = this.ActualWidth;
                double currentHeight = this.ActualHeight;

                // 创建宽度动画
                DoubleAnimation widthAnimation = new DoubleAnimation();
                widthAnimation.From = currentWidth;
                widthAnimation.To = newWidth;
                widthAnimation.Duration = TimeSpan.FromSeconds(0.5);
                widthAnimation.EasingFunction = easeOutExpo;

                // 创建高度动画
                DoubleAnimation heightAnimation = new DoubleAnimation();
                heightAnimation.From = currentHeight;
                heightAnimation.To = newHeight;
                heightAnimation.Duration = TimeSpan.FromSeconds(0.5);
                heightAnimation.EasingFunction = easeOutExpo;

                // 将动画添加到Storyboard
                Storyboard.SetTarget(widthAnimation, this);
                Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(Window.WidthProperty));

                Storyboard.SetTarget(heightAnimation, this);
                Storyboard.SetTargetProperty(heightAnimation, new PropertyPath(Window.HeightProperty));

                storyboard.Children.Add(widthAnimation);
                storyboard.Children.Add(heightAnimation);
            }

            storyboard.Begin();
            ToggleParticleEffect();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                var workArea = SystemParameters.WorkArea;
                MaxHeight = workArea.Height;
                MaxWidth = workArea.Width;
                Left = workArea.Left;
                Top = workArea.Top;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(this);
            // 如果鼠标在标题栏区域（例如，高度小于65），则允许拖动窗口
            if (mousePosition.Y < 65)
            {
                this.DragMove();
            }
        }
    }
}