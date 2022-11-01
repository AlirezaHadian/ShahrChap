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
        private GenericRepository<User_Address> user_AddressRepository;
        private GenericRepository<province> provinceRepository;
        private GenericRepository<city> cityRepository;
        private GenericRepository<School_Tools_Groups> school_Tools_GroupsRepository;
        private GenericRepository<Products> productsRepository;
        private GenericRepository<Product_Groups> product_GroupsRepository;
        private GenericRepository<Product_Galleries> product_GalleriesRepository;
        private GenericRepository<Tags> tagsRepository;
        private GenericRepository<Features> featuresRepository;
        private GenericRepository<Product_Features> product_FeaturesRepository;
        private GenericRepository<Factors> factorsRepository;
        private GenericRepository<Factor_Details> factor_DetailsRepository;
        private GenericRepository<Slider> sliderRepository;
        private GenericRepository<Order_Groups> order_GroupsRepository;
        private GenericRepository<Order_Details> order_DetailsRepository;
        private GenericRepository<Order_Files> order_FilesRepository;

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

        public GenericRepository<User_Address> User_AddressRepository
        {
            get
            {
                if (user_AddressRepository == null)
                {
                    user_AddressRepository = new GenericRepository<User_Address>(db);
                }

                return user_AddressRepository;
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
        public GenericRepository<School_Tools_Groups> School_Tools_GroupsRepository
        {
            get
            {
                if (school_Tools_GroupsRepository == null)
                {
                    school_Tools_GroupsRepository = new GenericRepository<School_Tools_Groups>(db);
                }

                return school_Tools_GroupsRepository;
            }
        }
        public GenericRepository<Products> ProductsRepository
        {
            get
            {
                if (productsRepository == null)
                {
                    productsRepository = new GenericRepository<Products>(db);
                }

                return productsRepository;
            }
        }
        public GenericRepository<Product_Groups> Product_GroupsRepository
        {
            get
            {
                if (product_GroupsRepository == null)
                {
                    product_GroupsRepository = new GenericRepository<Product_Groups>(db);
                }

                return product_GroupsRepository;
            }
        }
        public GenericRepository<Product_Galleries> Product_GalleriesRepository
        {
            get
            {
                if (product_GalleriesRepository == null)
                {
                    product_GalleriesRepository = new GenericRepository<Product_Galleries>(db);
                }

                return product_GalleriesRepository;
            }
        }
        public GenericRepository<Tags> TagsRepository
        {
            get
            {
                if (tagsRepository == null)
                {
                    tagsRepository = new GenericRepository<Tags>(db);
                }

                return tagsRepository;
            }
        }
        public GenericRepository<Features> FeaturesRepository
        {
            get
            {
                if (featuresRepository == null)
                {
                    featuresRepository = new GenericRepository<Features>(db);
                }

                return featuresRepository;
            }
        }
        public GenericRepository<Product_Features> Product_FeaturesRepository
        {
            get
            {
                if (product_FeaturesRepository == null)
                {
                    product_FeaturesRepository = new GenericRepository<Product_Features>(db);
                }

                return product_FeaturesRepository;
            }
        }
        public GenericRepository<Factors> FactorsRepository
        {
            get
            {
                if (factorsRepository == null)
                {
                    factorsRepository = new GenericRepository<Factors>(db);
                }

                return factorsRepository;
            }
        }
        public GenericRepository<Factor_Details> Factor_DetailsRepository
        {
            get
            {
                if (factor_DetailsRepository == null)
                {
                    factor_DetailsRepository = new GenericRepository<Factor_Details>(db);
                }

                return factor_DetailsRepository;
            }
        }
        public GenericRepository<Slider> SliderRepository
        {
            get
            {
                if (sliderRepository == null)
                {
                    sliderRepository = new GenericRepository<Slider>(db);
                }

                return sliderRepository;
            }
        }
        public GenericRepository<Order_Groups> Order_GroupsRepository
        {
            get
            {
                if (order_GroupsRepository == null)
                {
                    order_GroupsRepository = new GenericRepository<Order_Groups>(db);
                }

                return order_GroupsRepository;
            }
        }
        public GenericRepository<Order_Details> Order_DetailsRepository
        {
            get
            {
                if (order_DetailsRepository == null)
                {
                    order_DetailsRepository = new GenericRepository<Order_Details>(db);
                }

                return order_DetailsRepository;
            }
        }
        public GenericRepository<Order_Files> Order_FilesRepository
        {
            get
            {
                if (order_FilesRepository == null)
                {
                    order_FilesRepository = new GenericRepository<Order_Files>(db);
                }

                return order_FilesRepository;
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
