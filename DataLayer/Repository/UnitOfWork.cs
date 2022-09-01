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
        private GenericRepository<UserAddress> userAddressRepository;
        private GenericRepository<province> provinceRepository;
        private GenericRepository<city> cityRepository;

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

        public GenericRepository<UserAddress> UserAddressRepository
        {
            get
            {
                if (userAddressRepository == null)
                {
                    userAddressRepository = new GenericRepository<UserAddress>(db);
                }

                return userAddressRepository;
            }
        }
        public GenericRepository<province> ProvinceRepository
        {
            get
            {
                if (provinceRepository == null)
                {
                    provinceRepository = new GenericRepository<province>(db);
                }

                return provinceRepository;
            }
        }
        public GenericRepository<city> CityRepository
        {
            get
            {
                if (cityRepository == null)
                {
                    cityRepository = new GenericRepository<city>(db);
                }

                return cityRepository;
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
