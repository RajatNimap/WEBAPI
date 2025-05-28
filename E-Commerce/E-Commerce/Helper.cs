using AutoMapper;
using E_Commerce.Model;
using E_Commerce.Model.Entities;

namespace E_Commerce
{
    public class Helper:Profile
    {

        public Helper() {

            CreateMap<CategoryDto, Category>();

        }

    }
}
