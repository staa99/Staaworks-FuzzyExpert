using Staaworks.BankExpert.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Staaworks.BankExpert.WinForms.Interfaces
{
    public interface IQuestionOptionReciever
    {
        void SetQuestionValue(Question question, Option option);
    }
}
