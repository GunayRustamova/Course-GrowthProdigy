using FinalProject.Models;

namespace FinalProject.ViewModels
{
    public class Last30DaysVM
    {
        public IEnumerable<Expenditure> Expenditures { get; set; }
        public IEnumerable<Income> Incomes { get; set; }
        public IEnumerable<Student> Students { get; set; }
        public IEnumerable<Group> Groups { get; set; }
        public int StudentCount { get; set; }
        public float TotalExpenditureAmount { get; set; }
        public float TotalIncomeAmount { get; set; }
    } 
}
