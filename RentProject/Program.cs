using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using RentProject.Service;
using System.Configuration;
using RentProject.Repository;


namespace RentProject
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 建立容器，這是DI的註冊表
            var services = new ServiceCollection();

            // 1. 取得連線字串
            var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            // 2. 註冊 connectionString(讓repo使用)，Singleton = 整個程式生命週期只會建立一次，同一個物件一直重用。
            // 建議之後要修改
            services.AddSingleton<string>(connectionString);

            // 3. 註冊 repository
            services.AddSingleton<IRentTimeRepository>(sp => new DapperRentTimeRepository(sp.GetRequiredService<string>()));
            services.AddSingleton<IProjectRepository>(sp => new DapperProjectRepository(sp.GetRequiredService<string>()));
            services.AddSingleton<ITestLocationRepository>(sp => new DapperTestLocationRepository(sp.GetRequiredService<string>()));

            // 4. 註冊 services
            services.AddSingleton<IRentTimeService, RentTimeService>();
            services.AddSingleton<IProjectService, ProjectService>();
            services.AddSingleton<ITestLocationService, TestLocationService>();

            // 5. 註冊 Forms / UserControls
            services.AddTransient<Form1>(); // Form通常使用Transient，Transient = 每次跟容器要一次，就 new 一次新的。

            // Project 表單本體 (新增用：不帶 rentTimeId)
            services.AddTransient<Project>();

            // 工廠：編輯用 (帶 rentTimeId)
            // 這行會用 DI 把三個 service注入，在額外塞 rentTimeId 進去
            // 函式 Func<int, Project>，輸入int rentTimeId，輸出 Project；Func<輸入, 輸出>
            // Func<Project>：不用參數，回傳 Project（新增模式）
            // Func<int, string, Project>：輸入兩個參數，回傳 Project（更複雜的工廠）
            // sp：ServiceProvider(容器本體)
            // ActivatorUtilities.CreateInstance<Project>(sp, rentTimeId)：用 DI 幫你自動補齊 Project 建構子需要的服務 / 把你額外提供的 rentTimeId 也塞進去
            // 它會去找 Project 裡「最適合」的一個建構子，例如：public Project(RentTimeService a, ProjectService b, TestLocationService c, int rentTimeId)
            // a/b/c：DI 從 sp 取得
            services.AddTransient<Func<int, Project>>(sp =>
                rentTimeId => ActivatorUtilities.CreateInstance<Project>(sp, rentTimeId));

            var provider = services.BuildServiceProvider();

            // 用 DI 建立Form1 (Form1裡就不用 new 任何 service)
            // 不需要額外註冊，因為 DI 容器本身就能提供 IServiceProvider（只要你是用 provider.GetRequiredService<Form1>() 建立 Form1）。
            Application.Run(provider.GetRequiredService<Form1>());
        }
    }
}
