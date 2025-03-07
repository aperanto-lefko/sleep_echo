using sleepApp.Model;
using sleepApp.Service;

namespace sleepApp.Repository
{
    public class RespondentRepository
    {
        private readonly AppDbContext _context;

        public RespondentRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Respondent> GetAllRespondents()
        {
            var respondents = _context.Respondents.OrderBy(r => r.Id).ToList();
            return respondents;
        }

        public List<Respondent> GetRespondentByLastName(string lastName)
        {
            var respondents = _context.Respondents
                             .Where(r => r.LastName.ToLower().StartsWith(lastName.ToLower()))
                             .ToList();
            return respondents;
        }

        public Respondent AddRespondent(Respondent respondent)
        {
            _context.Respondents.Add(respondent);
            _context.SaveChanges();
            return respondent;
        }

        public int RemoveRespondent(Respondent respondent)
        {
            _context.Respondents.Remove(respondent);
            return _context.SaveChanges();
        }

        public bool UpdateRespondent(Respondent resp)
        {
            var oldRespondent = _context.Respondents.Find(resp.Id);
            if (oldRespondent == null)
            {
                return false;
            }
            _context.Entry(oldRespondent).CurrentValues.SetValues(resp);
            return _context.SaveChanges() > 0; //
        }

        public Respondent? FindById(int id)
        {
            return _context.Respondents.Find(id);
        }
    }
}