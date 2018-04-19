namespace Staaworks.BankExpert.WinForms
{
    partial class QuestionView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.OptionsContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.QuestionTextLabel = new System.Windows.Forms.RichTextBox();
            this.NextOrSubmitButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // OptionsContainer
            // 
            this.OptionsContainer.Location = new System.Drawing.Point(36, 69);
            this.OptionsContainer.Name = "OptionsContainer";
            this.OptionsContainer.Size = new System.Drawing.Size(319, 249);
            this.OptionsContainer.TabIndex = 1;
            // 
            // QuestionTextLabel
            // 
            this.QuestionTextLabel.BackColor = System.Drawing.SystemColors.Control;
            this.QuestionTextLabel.Font = new System.Drawing.Font("Lucida Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.QuestionTextLabel.Location = new System.Drawing.Point(36, 3);
            this.QuestionTextLabel.Name = "QuestionTextLabel";
            this.QuestionTextLabel.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.QuestionTextLabel.Size = new System.Drawing.Size(319, 60);
            this.QuestionTextLabel.TabIndex = 2;
            this.QuestionTextLabel.Text = "What is the distance of the nearest obstacle to the right of the car?";
            // 
            // NextOrSubmitButton
            // 
            this.NextOrSubmitButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.NextOrSubmitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.NextOrSubmitButton.Location = new System.Drawing.Point(149, 324);
            this.NextOrSubmitButton.Name = "NextOrSubmitButton";
            this.NextOrSubmitButton.Size = new System.Drawing.Size(75, 23);
            this.NextOrSubmitButton.TabIndex = 3;
            this.NextOrSubmitButton.UseVisualStyleBackColor = true;
            this.NextOrSubmitButton.Click += new System.EventHandler(this.Submit);
            // 
            // QuestionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.NextOrSubmitButton);
            this.Controls.Add(this.QuestionTextLabel);
            this.Controls.Add(this.OptionsContainer);
            this.Name = "QuestionView";
            this.Size = new System.Drawing.Size(393, 370);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel OptionsContainer;
        private System.Windows.Forms.RichTextBox QuestionTextLabel;
        private System.Windows.Forms.Button NextOrSubmitButton;
    }
}
