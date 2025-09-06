using METCSV.Common;
using System.Windows;
using UserControl = System.Windows.Controls.UserControl;

namespace METCSV.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for StepsControl.xaml
    /// </summary>
    public partial class StepsControl : UserControl
    {
        public StepsControl()
        {
            InitializeComponent();
        }



        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(OperationStatus),
            typeof(StepsControl), new FrameworkPropertyMetadata(OperationStatus.ReadyToStart, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(StatusChanged)));

        public OperationStatus Status
        {
            get { return (OperationStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        static void StatusChanged(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            ((StepsControl)property).ChangeControl((OperationStatus)args.NewValue);
        }

        private void ChangeControl(OperationStatus status)
        {
            GetStatus(status, out var waiting, out var progress, out var done, out var faild);

            IconInProgress.Visibility = progress;
            IconWaiting.Visibility = waiting;
            IconDone.Visibility = done;
            IconFaild.Visibility = faild;

            GetStatus(status, out waiting, out progress, out done, out faild);
        }

        private static void GetStatus(OperationStatus status, out Visibility waiting, out Visibility inProgress, out Visibility done, out Visibility faild)
        {
            switch (status)
            {
                case OperationStatus.ReadyToStart:
                    waiting = Visibility.Visible;
                    done = Visibility.Hidden;
                    inProgress = Visibility.Hidden;
                    faild = Visibility.Hidden;
                    break;
                case OperationStatus.InProgress:
                    waiting = Visibility.Hidden;
                    done = Visibility.Hidden;
                    inProgress = Visibility.Visible;
                    faild = Visibility.Hidden;
                    break;
                case OperationStatus.Complete:
                    waiting = Visibility.Hidden;
                    done = Visibility.Visible;
                    inProgress = Visibility.Hidden;
                    faild = Visibility.Hidden;
                    break;
                case OperationStatus.Faild:
                    waiting = Visibility.Hidden;
                    done = Visibility.Hidden;
                    inProgress = Visibility.Hidden;
                    faild = Visibility.Visible;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(status.ToString());
            }
        }

    }
}

