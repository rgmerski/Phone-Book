using Phone_Book.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace Phone_Book
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Contact> ContactsObs { get; set; }

        public MainWindow()
        {
            DataContext = this;

            InitializeComponent();

            DatabaseEntities db = new DatabaseEntities();
            var AllContacts = from d in db.Contacts 
                              select new
                              {
                                  Name = d.Name,
                                  Address = d.Address,
                                  Telephone = d.Telephone,
                                  Email = d.Email,
                                  Other = d.Other
                              };

            //db -> observable collection
            ContactsObs = new ObservableCollection<Contact>();
            // read from db as Contact
            foreach (var item in AllContacts.ToList())
            {
                ContactsObs.Add(new Contact()
                {
                    Name = item.Name,
                    Address = item.Address,
                    Email = item.Email,
                    Telephone = item.Telephone,
                    Other = item.Other,
                });
            }

            foreach (var item in AllContacts)
            {
                Console.WriteLine(item.Name);
                Console.WriteLine(item.Address);
            }

            data_grid.ItemsSource = ContactsObs;

        }

        // PROFILE (USER)
        private void LoadP_BTN_Click(object sender, RoutedEventArgs e)
        {
        }

        private void DeleteP_BTN_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SaveP_BTN_Click(object sender, RoutedEventArgs e)
        {
        }

        // CONTACT
        private void Save_BTN_Click(object sender, RoutedEventArgs e)
        {
            DatabaseEntities db = new DatabaseEntities();
            string mail = "mailto:";
            mail += Email_TXT.Text;
            //AllContacts.Add(new Contact(Name_TXT.Text, Address_TXT.Text, Telephone_TXT.Text, new Uri(mail), Other_TXT.Text));
            Contact temp = new Contact()
            {
                Name = Name_TXT.Text,
                Address = Address_TXT.Text,
                Telephone = Telephone_TXT.Text,
                Email = mail,
                Other = Other_TXT.Text
            };
            db.Contacts.Add(temp);
            db.SaveChanges();
            ContactsObs.Add(temp);
        }

        private void Delete_BTN_Click(object sender, RoutedEventArgs e)
        {
            DatabaseEntities db = new DatabaseEntities();
            Contact contact = data_grid.SelectedItem as Contact;

            if (contact != null) 
            {
                //var temp = from d in db.Contacts where d.Id == contact.Id select d;

                //Contact cont = temp.SingleOrDefault();


                //db.Contacts.Remove(cont);
                //ContactsObs.Remove(cont);
                db.Contacts.Attach(contact);
                db.Contacts.Remove(contact);
                ContactsObs.Remove(contact);
                db.SaveChanges();
            }
        }

        bool first = true;
        private void Edit_BTN_Click(object sender, RoutedEventArgs e)
        {
            Contact contact = data_grid.SelectedItem as Contact;
            if (first)
            {
                if (contact != null && first)
                {
                    Clear_BTN.IsEnabled = false;
                    Save_BTN.IsEnabled = false;
                    Delete_BTN.IsEnabled = false;
                    Update_BTN.IsEnabled = false;

                    Name_TXT.Text = contact.Name;
                    Telephone_TXT.Text = contact.Telephone;
                    Address_TXT.Text = contact.Address;
                    Other_TXT.Text = contact.Other;
                    Email_TXT.Text = contact.Email;
                    MessageBox.Show("Write all needed changes and click Edit Button again.");
                    first = false;
                }

            }
            else
            {
                EditRecord(contact.Id);
            }
            
        }

        private void EditRecord(int id)
        {
            DatabaseEntities db = new DatabaseEntities();

            Clear_BTN.IsEnabled = true;
            Save_BTN.IsEnabled = true;
            Delete_BTN.IsEnabled = true;
            Update_BTN.IsEnabled = true;

            var temp = from d in db.Contacts where d.Id == id select d;

            Contact cont = temp.SingleOrDefault();

            if (cont != null)
            {
                cont.Name = Name_TXT.Text;
                cont.Telephone = Telephone_TXT.Text;
                cont.Address = Address_TXT.Text;
                cont.Email = Email_TXT.Text;
                cont.Other = Other_TXT.Text;
            }


            MessageBox.Show("Record is updated!");
            first = true;
            db.SaveChanges();

            // Update in obs coll
            //var find = from d in ContactsObs where d.Id == id select d;
            //Contact cont2 = find.SingleOrDefault();

            //if (cont2 != null)
            //{
            //    cont2.Name = Name_TXT.Text;
            //    cont2.Telephone = Telephone_TXT.Text;
            //    cont2.Address = Address_TXT.Text;
            //    cont2.Email = Email_TXT.Text;
            //    cont2.Other = Other_TXT.Text;
            //}

            Name_TXT.Text = "";
            Telephone_TXT.Text = "";
            Address_TXT.Text = "";
            Other_TXT.Text = "";
            Email_TXT.Text = "";
        }

        private void Clear_BTN_Click(object sender, RoutedEventArgs e)
        {
            DatabaseEntities db = new DatabaseEntities();
            var AllContacts = from d in db.Contacts
                              select d;

            //db.Contacts.Remove();
        }

        private void Search_TXT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
            }
        }

        private void data_grid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string headername = e.Column.Header.ToString();
            if (headername == "Id" || headername == "UserID") e.Cancel = true;
        }

        private void Update_BTN_Click(object sender, RoutedEventArgs e)
        {
            DatabaseEntities db = new DatabaseEntities();
            data_grid.ItemsSource = db.Contacts.ToList();
        }
    }
}