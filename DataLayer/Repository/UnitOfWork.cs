using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Context
{
    public class UnitOfWork:IDisposable
    {
        ShahrChap_DBEntities db = new ShahrChap_DBEntities();

        private GenericRepository<User> userRepository;
        private GenericRepository<Role> roleRepository;

        public GenericRepository<User> UserRepository
        {
            get
            {
                if (userRepository == null)
                {
                    userRepository=new GenericRepository<User>(db);
                }

                return userRepository;
            }
        }

        public GenericRepository<Role> RoleRepository
        {
            get
            {
                if (roleRepository == null)
                {
                    roleRepository = new GenericRepository<Role>(db);
                }

                return roleRepository;
            }
        }

        public void Save()
        {
            db.SaveChanges();
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
