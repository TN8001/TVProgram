using System.Windows;
using System.Windows.Interop;

namespace TVProgram
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MouseLeftButtonDown += (s, e) => DragMove();
            MouseRightButtonUp += (s, e) => Close();

            //擦りガラス効果 環境によっては動かないかもしれません（Win10のみ？？）
            //動かない場合はコメントアウトしてください
            Loaded += (s, e) => Win32Helper.EnableBlur(new WindowInteropHelper(this).Handle);
        }
    }
}
