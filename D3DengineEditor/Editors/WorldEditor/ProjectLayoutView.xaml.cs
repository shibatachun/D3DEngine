﻿
using System.Windows;
using System.Windows.Controls;
using D3DengineEditor.GameProject;
using D3DengineEditor.Components;
using D3DengineEditor.Utilities;
namespace D3DengineEditor.Editors
{
    /// <summary>
    /// Interaction logic for ProjectLayoutView.xaml
    /// </summary>
    public partial class ProjectLayoutView : UserControl
    {
        public ProjectLayoutView()
        {
            InitializeComponent();
        }

        private void OnAddGameEntity_Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var vm = btn.DataContext as Scene;
            vm.AddGameEntityCommand.Execute(new GameEntity(vm) { Name = "Empty Game Entity" });
        }

        private void OnGameEntities_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
     

            //What selections before
            var listBox = sender as ListBox;

            var newSelection = listBox.SelectedItems.Cast<GameEntity>().ToList();
            var previousSelection = newSelection.Except(e.AddedItems.Cast<GameEntity>()).Concat(e.RemovedItems.Cast<GameEntity>()).ToList();

            Project.UndoRedo.Add(new UndoRedoAction(
                () =>
                {
                    listBox.UnselectAll();
                    previousSelection.ForEach(x=>(listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem).IsSelected = true);
                },

                () =>
                 {
                     listBox.UnselectAll();
                     newSelection.ForEach(x => (listBox.ItemContainerGenerator.ContainerFromItem(x) as ListBoxItem).IsSelected = true);
                 },

                "Selection chagned"
                ));

            MSGameEntity msEntity = null;
            if(newSelection.Any())
            {
                msEntity = new MSGameEntity(newSelection);
                listBox.Focus();

            }
            GameEntityView.Instance.DataContext = msEntity; 
          
           
        }
    }
}
