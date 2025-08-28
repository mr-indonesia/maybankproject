using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedKernel.Interfaces;

namespace App.Core.Interfaces
{
    public interface ISchedulerService : IRepository
    {
        void Run();
    }
}
