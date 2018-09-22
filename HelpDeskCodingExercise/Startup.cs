using HelpDeskCodingExercise.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HelpDeskCodingExercise.Startup))]
namespace HelpDeskCodingExercise
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandSuperUser();
        }

        // In this method we will create default User roles and Admin user for login   
        private void createRolesandSuperUser()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            //NOTE: role manager to create the basic roles if not present in the database.
            // Probably would be best in a separate controller specifically for role based functions.
            //Create the default roles. Could be better as a role controller.
            foreach (string roles in System.Enum.GetNames(typeof(CustomUserRoles)))
            {
                if (!roleManager.RoleExists(roles))
                {
                    var role = new IdentityRole();
                    role.Name = roles;
                    roleManager.Create(role);
                }
            }

            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (roleManager.RoleExists("admin"))
            {
                //Here we create an Admin super user who will maintain the website   
                var superAdmin = new ApplicationUser();
                superAdmin.UserName = "admin@f23.com";
                superAdmin.Email = superAdmin.UserName;

                string adminPassword = "Webb1!";

                var chkUser = UserManager.Create(superAdmin, adminPassword);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    var result = UserManager.AddToRole(superAdmin.Id, "Admin");

                }
            }
        }
    }
}
