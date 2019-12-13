﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UNA.NoteBook;
using System.Windows.Forms;

namespace NoteBook
{
    public partial class NoteBookForm : Form
    {
        List<User> users= new List<User>();
        Dictionary<int, string> directionImages = new Dictionary<int, string>();  
        bool isLogin = false;
        User actualSesion = null;
        public NoteBookForm()
        {
            InitializeComponent();
            preloadImages();
        }

        private void signUpButton_Click(object sender, EventArgs e)
        {
            NoteBookRegisterForm noteBookRegister = new NoteBookRegisterForm(users);
            if(noteBookRegister.ShowDialog() == DialogResult.OK)
            {
                users.Add(noteBookRegister.NewUser);
                MessageBox.Show("Usuario Creado Exitosamente","Felicidades");
                isLogin = true;
                userSingInLabel.Text = "<" + (string)noteBookRegister.NewUser.NameUser + ">";
                signOutButton.Enabled = true;
            }
        }

        private void logInButton_Click(object sender, EventArgs e)
        {
            if(users.Count != 0 && isLogin == false)
            {
                NoteBookSignInForm noteBookSignInForm = new NoteBookSignInForm(users);
                if (noteBookSignInForm.ShowDialog() == DialogResult.OK)
                {
                    isLogin = true;
                    actualSesion = noteBookSignInForm.User;
                    MessageBox.Show("'"+actualSesion.NameUser+"' A Iniciado Sesion" , "Inicio de Sesion");
                    userSingInLabel.Text = "<" + actualSesion.NameUser + ">";
                    signOutButton.Enabled = true;
                }
            }
            else if(users.Count == 0)
            {
                MessageBox.Show("No Hay Usuarios Registrados", "Error");
            }
            else if(isLogin == true)
            {
                NotebookProfileForm notebookProfileForm = new NotebookProfileForm();
                {
                    if(notebookProfileForm.ShowDialog() == DialogResult.OK)
                    {
                    }
                }
            }
        }
        private void signOutButton_Click(object sender, EventArgs e)
        {
            isLogin = false;
            actualSesion = null;
            userSingInLabel.Text = "<No Autentificado>";
             signOutButton.Enabled = false;
        }

        private void createBookButton_Click(object sender, EventArgs e)
        {
            //if(libraryTableLayoutPanel.GetControlFromPosition(0,0) != null)
            //{

            //}
            NoteBookNewBookForm noteBookNewBookForm = new NoteBookNewBookForm(directionImages);
            if(noteBookNewBookForm.ShowDialog() == DialogResult.OK)
            {
                PictureBox pictureBox = new PictureBox();
                Console.WriteLine(noteBookNewBookForm.NewBook.ImageBook);
                pictureBox.ImageLocation = noteBookNewBookForm.NewBook.ImageBook;
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox.Width = 45;
                pictureBox.Height = 45;
                pictureBox.Anchor = AnchorStyles.None;
                ToolTip toolTip = new ToolTip();
                toolTip.ToolTipTitle = noteBookNewBookForm.NewBook.NameBook;
                toolTip.SetToolTip(pictureBox, "Categoria: " + noteBookNewBookForm.NewBook.CategorieBook);
                toolTip.IsBalloon = true;
                libraryTableLayoutPanel.Controls.Add(pictureBox);
            }
        }

        private void timeTimer_Tick(object sender, EventArgs e)
        {
            timeLabel.Text = DateTime.Now.ToString("h:mm:ss tt");
        }

        private void preloadImages()
        {
            directionImages.Add(0, @"Resource\Deportes.png");
            directionImages.Add(1, @"Resource\Peliculas.png");
            directionImages.Add(2, @"Resource\Juegos.png");
            directionImages.Add(3, @"Resource\Musica.png");
            directionImages.Add(4, @"Resource\Libros.png");
            directionImages.Add(5, @"Resource\Artes.png");
        }
    }
}
