namespace NsDemo
{
    partial class DataTossing
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dateTimePicker_Tossing = new System.Windows.Forms.DateTimePicker();
            this.chartTossing = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartTossing)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85.57457F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.42543F));
            this.tableLayoutPanel1.Controls.Add(this.dateTimePicker_Tossing, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.chartTossing, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 94.83334F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.166667F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1227, 600);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // dateTimePicker_Tossing
            // 
            this.dateTimePicker_Tossing.Location = new System.Drawing.Point(40, 572);
            this.dateTimePicker_Tossing.Margin = new System.Windows.Forms.Padding(40, 3, 3, 3);
            this.dateTimePicker_Tossing.Name = "dateTimePicker_Tossing";
            this.dateTimePicker_Tossing.Size = new System.Drawing.Size(200, 21);
            this.dateTimePicker_Tossing.TabIndex = 0;
            this.dateTimePicker_Tossing.ValueChanged += new System.EventHandler(this.dateTimePicker_Tossing_ValueChanged);
            // 
            // chartTossing
            // 
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisX.MajorTickMark.Interval = 1D;
            chartArea1.AxisX.MajorTickMark.IntervalOffset = 1D;
            chartArea1.AxisX.MajorTickMark.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Days;
            chartArea1.AxisX.ScaleBreakStyle.StartFromZero = System.Windows.Forms.DataVisualization.Charting.StartFromZero.No;
            chartArea1.AxisX2.Title = "xxx";
            chartArea1.AxisY.Interval = 50D;
            chartArea1.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.AxisY.Maximum = 500D;
            chartArea1.AxisY.Minimum = 0D;
            chartArea1.Name = "ChartArea1";
            this.chartTossing.ChartAreas.Add(chartArea1);
            this.chartTossing.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            legend1.Title = "啊啊啊";
            this.chartTossing.Legends.Add(legend1);
            this.chartTossing.Location = new System.Drawing.Point(3, 3);
            this.chartTossing.Name = "chartTossing";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.IsValueShownAsLabel = true;
            series1.LabelBackColor = System.Drawing.Color.AliceBlue;
            series1.LabelBorderColor = System.Drawing.Color.LightBlue;
            series1.LabelBorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            series1.Legend = "Legend1";
            series1.MarkerSize = 8;
            series1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series1.Name = "Series1";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Date;
            series1.YValuesPerPoint = 6;
            this.chartTossing.Series.Add(series1);
            this.chartTossing.Size = new System.Drawing.Size(1043, 563);
            this.chartTossing.TabIndex = 1;
            this.chartTossing.Text = "chartPress";
            title1.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            title1.ForeColor = System.Drawing.SystemColors.WindowFrame;
            title1.Name = "Title1";
            title1.Text = "Weekly Tossing Trend";
            this.chartTossing.Titles.Add(title1);
            // 
            // DataTossing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1227, 600);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DataTossing";
            this.Text = "DataTossing";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartTossing)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DateTimePicker dateTimePicker_Tossing;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartTossing;

    }
}