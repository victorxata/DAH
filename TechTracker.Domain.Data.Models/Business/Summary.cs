using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MongoDB.Bson;
using TechTracker.Domain.Data.Core.MongoDb;

namespace TechTracker.Domain.Data.Models.Business
{
    [DataContract]
    public class Summary : Entity
    {

        [DataMember]
        public int Year { get; set; }

        [DataMember]
        public int Target { get; set; }

        [DataMember]
        public int Hired { get; set; }

        [DataMember]
        public List<ByAccount> ByAccount { get; set; }

        [DataMember]
        public List<BySkill> BySkill { get; set; }

        public Summary()
        {
            ByAccount = new List<ByAccount>();
            BySkill = new List<BySkill>();
            if (Id == null)
                Id = ObjectId.GenerateNewId().ToString();
        }

        public Summary AddSkill(string skillId, string skillName)
        {
            var exists = this.BySkill.Count(x => x.SkillId == skillId);
            if (exists > 0) return this;
            this.BySkill.Add(new BySkill(skillId, skillName));
            return this;
        }

        public Summary UpdateSkill(string skillId, string skillName)
        {
            var skill = this.BySkill.FirstOrDefault(x => x.SkillId == skillId);
            skill.SkillName = skillName;
            return this;
        }

        public Summary RemoveSkill(string skillId)
        {
            this.BySkill.RemoveAll(x => x.SkillId == skillId);
            return this;
        }

        public Summary AddAccount(string accountId, string accountName)
        {
            var exists = this.ByAccount.Count(x => x.AccountId == accountId);
            if (exists > 0) return this;
            this.ByAccount.Add(new ByAccount(accountId, accountName));
            return this;
        }

        public Summary RemoveAccount(string accountId)
        {
            this.ByAccount.RemoveAll(x => x.AccountId == accountId);
            return this;
        }

        public Summary UpdateAccount(string accountId, string accountName)
        {
            var account = this.ByAccount.FirstOrDefault(x => x.AccountId == accountId) ??
                          new ByAccount(accountId, accountName);

            account.AccountName = accountName;
            this.ByAccount.RemoveAll(x => x.AccountId == accountId);
            this.ByAccount.Add(account);
            return this;
        }

        public Summary AddRole(string roleId, string accountId, string accountName, List<Skill> skills)
        {
            var account = this.ByAccount.FirstOrDefault(x => x.AccountId == accountId) ??
                          new ByAccount(accountId, accountName);

            account.Target = account.Target + 1;
            this.ByAccount.RemoveAll(x => x.AccountId == accountId);
            this.ByAccount.Add(account);

            foreach (var skill in skills)
            {
                var ski = this.BySkill.FirstOrDefault(x => x.SkillId == skill.Id) ??
                          new BySkill(skill.Id, skill.Description);

                ski.Target = ski.Target + 1;
                this.BySkill.RemoveAll(x => x.SkillId == skill.Id);
                this.BySkill.Add(ski);
            }

            return this;
        }

        public Summary IncreaseTarget(string skillId, string accountId, int quantity)
        {
            var summaryByAccount = this.ByAccount.FirstOrDefault(x => x.AccountId == accountId);
            summaryByAccount.Target = summaryByAccount.Target + quantity;
            this.RemoveAccount(accountId);
            this.ByAccount.Add(summaryByAccount);

            var summaryBySkill = this.BySkill.FirstOrDefault(x => x.SkillId == skillId);
            if (summaryBySkill == null) return this;
            summaryBySkill.Target = summaryBySkill.Target + quantity;
            this.RemoveSkill(skillId);
            this.BySkill.Add(summaryBySkill);
            return this;
        }
    }

    public class ByAccount
    {
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public int Target { get; set; }
        public int Hired { get; set; }

        public ByAccount(string accountId, string accountName)
        {
            AccountId = accountId;
            AccountName = accountName;
            Target = 0;
            Hired = 0;
        }
    }

    public class BySkill
    {
        public string SkillId { get; set; }
        public string SkillName { get; set; }
        public int Target { get; set; }
        public int Hired { get; set; }

        public BySkill(string skillId, string skillName)
        {
            SkillId = skillId;
            SkillName = skillName;
            Target = 0;
            Hired = 0;
        }
    }
}
