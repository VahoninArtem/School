using System;
using System.Collections.Generic;
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

namespace School
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Client> clients = new List<Client>();
        public MainWindow()
        {
            InitializeComponent();
            FillTable();
        }
        private void FillTable ()
        {
          using (SchoolContainer db = new SchoolContainer())
            {
                var clients = db.Client.Join(db.Gender,
                    p => p.GenderCode,
                    q => q.Code,
                    (p, q) => new
                    {
                        Id = p.ID,
                        Gender = q.Name,
                        Name = p.FirstName,
                        LastName = p.LastName,
                        Patronymic = p.Patronymic,
                        DateBirth = p.Birthday,
                        Phone = p.Phone,
                        Email = p.Email,
                        DateAdd = p.RegistrationDate
                    }).ToList();

                var list = (from c in db.Client
                    join v in db.Visiting on c.ID equals v.IDClient
                    into gj
                    from q in gj.DefaultIfEmpty()
                    select new
                    {
                        Id = c.ID,
                        DateLastVisit = q.Date == null ? null : q.Date,
                        Quantity = q.Date == null ? 0:1
                       }).GroupBy(p=>p.Id).Select(q=> new { Id=q.Key, Count = q.Sum(w=>w.Quantity),
                           Date = q.Max(w=>w.DateLastVisit)}).ToList();

                var result = (from c in clients
                              from l in list
                              where c.Id == l.Id
                              select new
                              {
                                  Id = c.Id,
                                  Gender = c.Gender,
                                  Name = c.Name,
                                  LastName = c.LastName,
                                  Patronymic = c.Patronymic,
                                  DataBirth = c.DateBirth,
                                  Phone = c.Phone,
                                  Email = c.Email,
                                  DateAdd = c.DateAdd,
                                  DateLastVisit = l.Date,
                                  Count = l.Count
                              }).ToList();

                DataGridClients.ItemsSource = result;
                }
            }
        }
    }
