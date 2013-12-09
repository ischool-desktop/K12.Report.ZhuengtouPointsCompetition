using FISCA.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K12.Report.ZhuengtouPointsCompetition.DAO
{
    /// <summary>
    /// 使用 FISCA.Data Query
    /// </summary>
    public class FDQuery
    {
        public const string _StudentSatus = "1";

        /// <summary>
        /// 取得學生曾經參與過的社團的學年度學期
        /// </summary>
        /// <param name="StudentIdList"></param>
        /// <returns></returns>
        public static Dictionary<string, ValueObj.ClubsVO> GetClubRecordByStudentIdList(List<string> StudentIdList)
        {
            Dictionary<string, ValueObj.ClubsVO> result = new Dictionary<string,ValueObj.ClubsVO>();
            StringBuilder sb = new StringBuilder();
            sb.Append("select t1.school_year, t1.semester, t2.ref_student_id");
            sb.Append(" from $k12.clubrecord.universal t1, $k12.scjoin.universal t2");
            sb.Append(" where t2.ref_club_id::int = t1.uid");
            sb.Append(" and t2.ref_student_id in ('" + string.Join("','", StudentIdList.ToArray()) + "')");
            // sb.Append(" and student.status = '" + _StudentSatus + "'");
            sb.Append(" and t1.school_year is not NULL");
            sb.Append(" and t1.semester is not NULL");

            if (Global.IsDebug) Console.WriteLine("[GetClubRecordByStudentIdList] sql: [" + sb.ToString() + "]");
            
            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(sb.ToString());

            foreach (DataRow row in dt.Rows)
            {
                string studentId = ("" + row["ref_student_id"]).Trim();
                if(!result.ContainsKey(studentId))
                    result.Add(studentId, new ValueObj.ClubsVO());

                result[studentId].AddClub(row);
            }

            return result;
        }

        /// <summary>
        /// 取得學生各學期的服務學習時數
        /// </summary>
        /// <param name="StudentIdList"></param>
        /// <returns></returns>
        public static Dictionary<string, ValueObj.ServicesVO> GetLearningServiceByStudentIdList(List<string> StudentIdList)
        {
            Dictionary<string, ValueObj.ServicesVO> result = new Dictionary<string,ValueObj.ServicesVO>();
            StringBuilder sb = new StringBuilder();
            sb.Append("select ref_student_id, school_year, semester, hours, occur_date");
            sb.Append(" from $k12.service.learning.record");
            sb.Append(" where ref_student_id in ('" + string.Join("','", StudentIdList.ToArray()) + "')");
            // sb.Append(" and student.status = '" + _StudentSatus + "'");
            sb.Append(" and school_year is not NULL");
            sb.Append(" and semester is not NULL");

            if (Global.IsDebug) Console.WriteLine("[GetLearningServiceByStudentIdList] sql: [" + sb.ToString() + "]");

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(sb.ToString());

            foreach (DataRow row in dt.Rows)
            {
                string studentId = ("" + row["ref_student_id"]).Trim();

                if(!result.ContainsKey(studentId))
                    result.Add(studentId, new ValueObj.ServicesVO());
                
                result[studentId].AddService(row);
            }

            return result;
        }

        /// <summary>
        /// 取得學生資料
        /// </summary>
        /// <param name="StudentIdList"></param>
        /// <returns></returns>
        public static Dictionary<string, ValueObj.StudentVO> GetStudentInfo(List<string> StudentIdList)
        {
            Dictionary<string, ValueObj.StudentVO> result = new Dictionary<string,ValueObj.StudentVO>();
            StringBuilder sb = new StringBuilder();
            sb.Append("select student.id as student_id,");
            sb.Append("student.name as student_name,");
            sb.Append("student.birthdate as student_birthday,");
            sb.Append("student.id_number as student_idnumber,");
            sb.Append("tag_student.ref_tag_id");
            sb.Append(" from student");
            sb.Append(" left join tag_student");
            sb.Append(" on tag_student.ref_student_id = student.id");
            sb.Append(" where student.id in ('" + string.Join("','", StudentIdList.ToArray()) + "')");
            // sb.Append(" and student.status = '" + _StudentSatus + "'");

            if (Global.IsDebug) Console.WriteLine("[GetStudentInfo] sql: [" + sb.ToString() + "]");

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(sb.ToString());

            foreach (DataRow row in dt.Rows)
            {
                string studentId = ("" + row["student_id"]).Trim();
                if(!result.ContainsKey(studentId))
                {
                    ValueObj.StudentVO studentVO = new ValueObj.StudentVO(row);
                    result.Add(studentId, studentVO);
                }
                else
                {
                    result[studentId].AddTagId(row);
                }
            }

            return result;
        }

        /// <summary>
        /// 取得學生領域成績
        /// </summary>
        /// <param name="StudentIdList"></param>
        /// <returns></returns>
        public static Dictionary<string, ValueObj.DomainsVO> GetDomainScore(List<string> StudentIdList)
        {
            // 學生每個學年度學期的領域分數
            Dictionary<string, ValueObj.DomainsVO> result = new Dictionary<string,ValueObj.DomainsVO>();
            List<JHSchool.Data.JHSemesterScoreRecord> SemesterScoreList = JHSchool.Data.JHSemesterScore.SelectByStudentIDs(StudentIdList);

            foreach (JHSchool.Data.JHSemesterScoreRecord rec in SemesterScoreList)
            {
                string studentId = rec.RefStudentID;
                if(!result.ContainsKey(studentId))
                    result.Add(studentId, new ValueObj.DomainsVO());
                
                foreach(Data.DomainScore domainScore in rec.Domains.Values)
                {
                    ValueObj.SchoolYearSemester SchoolYearSemester = new ValueObj.SchoolYearSemester(domainScore.SchoolYear, domainScore.Semester);
                    ValueObj.DomainsVO domainsVo = result[studentId];
                    domainsVo.AddDomain(SchoolYearSemester, studentId, domainScore.Domain, domainScore.Score);
                }
            }

            return result;
        }   // end of GetDomainScore

        /// <summary>
        /// 取的系統內所有的類別
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetSysTag()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            StringBuilder sb = new StringBuilder();
            sb.Append("select id, prefix, name from tag where category='Student' order by prefix,name");

            if (Global.IsDebug) Console.WriteLine("[GetAllTag] sql: [" + sb.ToString() + "]");

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(sb.ToString());

            foreach (DataRow row in dt.Rows)
            {
                String id = ("" + row["id"]).Trim();
                String prefix = ("" + row["prefix"]).Trim();
                String name = ("" + row["name"]).Trim();
                if (!result.ContainsKey(id))
                {
                    result.Add(Utility.GetTagName(prefix, name), id);
                }
            }

            return result;
        }
    }
}
