using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using D3DengineEditor.GameDev;
using D3DengineEditor.GameProject;
using D3DengineEditor.Utilities;
using Xceed.Wpf.Toolkit;

namespace D3DengineEditor.Editors
{
    /// <summary>
    /// Interaction logic for WorldEditorView.xaml
    /// </summary>
    public partial class WorldEditorView : UserControl
    {
        public WorldEditorView()
        {
            InitializeComponent();
            Loaded += OnWorldEditorViewLoaded;
            //Loaded += LoadedFromLayout;
            //Unloaded += SaveToLayout;
        }

      

        private void OnWorldEditorViewLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnWorldEditorViewLoaded;
            Focus();
            //有时候会导致整个界面lost focus，这个是保持整个editor有Focus
            //((INotifyCollectionChanged)Project.UndoRedo.UndoList).CollectionChanged += (s, e) => Focus();
        }

        private void GameEntityView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void OnNewSCript_Button_Click(object sender, RoutedEventArgs e)
        {
            new NewScriptDialog().ShowDialog();
          
        }

        //private void SaveToLayout(object sender, RoutedEventArgs e)
        //{
        //    Unloaded -= SaveToLayout;
        //    DockManager.UpdateLayout();
        //    using (var stream = new StreamWriter("layout.xml"))
        //    {
        //        var layoutSerializer = new Xceed.Wpf.AvalonDock.Layout.Serialization.XmlLayoutSerializer(DockManager);
        //        layoutSerializer.Serialize(stream);
        //    }
        //}
        //private void LoadedFromLayout(object sender, RoutedEventArgs e)
        //{
        //    if(File.Exists("layout.xml"))
        //    {
        //        Console.WriteLine(Path.GetFullPath("layout.xml"));

        //        using (var stream = new StreamReader("layout.xml"))

        //        {
        //            var layoutSerializer = new Xceed.Wpf.AvalonDock.Layout.Serialization.XmlLayoutSerializer(DockManager);
        //            layoutSerializer.LayoutSerializationCallback += (s, args) =>
        //            {

        //            };
        //            layoutSerializer.Deserialize(stream);
        //        }
        //    }
        //}
    }
}
