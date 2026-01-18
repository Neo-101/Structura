using System.Collections.Generic;
using System.Windows;

namespace Structura.UI
{
    public partial class DepthDistributionWindow : Window
    {
        public DepthDistributionWindow(List<DepthStatItem> depthStats, List<BreadthStatItem> breadthStats)
        {
            InitializeComponent();
            StatsGrid.ItemsSource = depthStats;
            BreadthGrid.ItemsSource = breadthStats;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}