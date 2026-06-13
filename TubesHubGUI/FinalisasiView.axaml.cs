using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using TubesHubGUI;

namespace TubesHubGUI
{
    public partial class FinalisasiView : UserControl
    {
        public FinalisasiView()
        {
            InitializeComponent();
            LoadStatus(); // Langsung load JSON saat halaman dibuka
        }

        private void LoadStatus()
        {
            try
            {
                var status = FinalisasiModule.GetLaporanStatus();
                string displayText = "";
                foreach (var item in status)
                {
                    displayText += $"{item.Key}: {item.Value}\n";
                }
                StatusTextBlock.Text = displayText;
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = "Gagal memuat data.";
                PesanTextBlock.Text = ex.Message;
                PesanTextBlock.Foreground = Brushes.Red;
            }
        }

        private void OnSimpanClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedBab = (BabComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                var selectedStatus = (StatusComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

                if (selectedBab != null && selectedStatus != null)
                {
                    // Panggil logika utama C#
                    FinalisasiModule.UbahStatusDokumen(selectedBab, selectedStatus);

                    // Update UI jika berhasil
                    PesanTextBlock.Foreground = Brushes.Green;
                    PesanTextBlock.Text = $"[INFO] Status {selectedBab} berhasil diubah menjadi {selectedStatus}!";

                    LoadStatus(); // Refresh teks status dokumen di layar
                }
            }
            catch (Exception ex)
            {
                // Jika DbC / Automata menolak, tampilkan error warna merah di GUI
                PesanTextBlock.Foreground = Brushes.Red;
                PesanTextBlock.Text = ex.Message;
            }
        }
    }
}