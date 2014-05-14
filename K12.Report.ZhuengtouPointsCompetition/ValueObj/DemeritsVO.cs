using K12.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Report.ZhuengtouPointsCompetition.ValueObj
{
    public class DemeritsVO
    {

        private Dictionary<SchoolYearSemester, List<AutoSummaryRecord>> DemeritsBySchoolYear = new Dictionary<SchoolYearSemester, List<AutoSummaryRecord>>();

        public void AddDemerit(AutoSummaryRecord rec)
        {
            ValueObj.SchoolYearSemester SchoolYearSemester = new ValueObj.SchoolYearSemester(rec.SchoolYear, rec.Semester);
            if (!DemeritsBySchoolYear.ContainsKey(SchoolYearSemester))
                DemeritsBySchoolYear.Add(SchoolYearSemester, new List<AutoSummaryRecord>());
            DemeritsBySchoolYear[SchoolYearSemester].Add(rec);
        }

        public List<AutoSummaryRecord> GetDemeritsBySchoolYear(SchoolYearSemester schoolYearSemester)
        {
            if (DemeritsBySchoolYear.ContainsKey(schoolYearSemester))
                return DemeritsBySchoolYear[schoolYearSemester];
            else
                return new List<AutoSummaryRecord>();
        }
        //private Dictionary<SchoolYearSemester, List<Data.DemeritRecord>> DemeritsBySchoolYear = new Dictionary<SchoolYearSemester,List<Data.DemeritRecord>>();

        //public void AddDemerit(Data.DemeritRecord rec)
        //{

        //    // 已銷過的不處理
        //    if (rec.Cleared == "是") return;

        //    // 非懲戒的不處理
        //    if (rec.MeritFlag != "0") return;

        //    ValueObj.SchoolYearSemester SchoolYearSemester = new ValueObj.SchoolYearSemester(rec.SchoolYear, rec.Semester);
        //    if (!DemeritsBySchoolYear.ContainsKey(SchoolYearSemester))
        //        DemeritsBySchoolYear.Add(SchoolYearSemester, new List<Data.DemeritRecord>());
        //    DemeritsBySchoolYear[SchoolYearSemester].Add(rec);
        //}

        //public List<Data.DemeritRecord> GetDemeritsBySchoolYear(SchoolYearSemester schoolYearSemester)
        //{
        //    if (DemeritsBySchoolYear.ContainsKey(schoolYearSemester))
        //        return DemeritsBySchoolYear[schoolYearSemester];
        //    else
        //        return new List<Data.DemeritRecord>();
        //}
    }
}
