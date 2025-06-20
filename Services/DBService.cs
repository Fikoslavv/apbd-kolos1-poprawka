using System.Data.Common;
using apbd_kolos1_poprawka.Models.DTOs;
using apbd_kolos1_poprawka.Models.RequestDTOs;
using Microsoft.Data.SqlClient;

namespace apbd_kolos1_poprawka.Services;

public class DBService : IDBService
{
    protected const string GET_PRESERVATION_PROJECT_BY_ID_SQL_QUERRY = "select pp.projectId as p_project_id, startDate as p_start_date, endDate as p_end_date, ar.artifactId as ar_artifact_id, objective as p_objective, sa.role as sa_role, st.staffId as s_staff_id, st.firstName as s_first_name, st.lastName as s_last_name, st.hireDate as s_hire_date, ar.name as a_name, ar.originDate as a_origin_date, it.institutionId as i_institution_id, it.name as i_name, it.foundedYear as i_founded_year from preservation_project pp left join staff_assignment sa on sa.projectId = pp.projectId left join staff st on st.staffId = sa.staffId join artifact ar on ar.artifactId = pp.artifactId join institution it on it.institutionId = ar.institutionId where pp.projectId = @projectId";

    protected const string INSERT_NEW_ARTIFACT_QUERRY = "insert into artifact values (@artifact_id, @artifact_name, @artifact_origin_date, @artifact_institution_id);";
    protected const string INSERT_NEW_PRESERVATION_PROJECT_QUERRY = "insert into preservation_project values (@project_id, @artifact_id, @project_start_date, @project_end_date, @project_objective);";
    protected string connectionString;

    public DBService(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public async Task<PreservationProjectDTO> GetPreservationProjectByIdAsync(int projectId)
    {
        PreservationProjectDTO project;

        using (SqlConnection connection = new(this.connectionString))
        using (SqlCommand command = new(GET_PRESERVATION_PROJECT_BY_ID_SQL_QUERRY, connection))
        {
            await connection.OpenAsync();

            command.Parameters.AddWithValue("@projectId", projectId);

            try
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    ArtifactDTO artifact;
                    InstitutionDTO institution;

                    await reader.ReadAsync();

                    var projectIdOridinal = reader.GetOrdinal("p_project_id");
                    var artifactIdOridinal = reader.GetOrdinal("ar_artifact_id");
                    var projectStartDateOridinal = reader.GetOrdinal("p_start_date");
                    var projectEndDateOridinal = reader.GetOrdinal("p_end_date");
                    var projectObjectiveOridinal = reader.GetOrdinal("p_objective");
                    var artifactNameOridinal = reader.GetOrdinal("a_name");
                    var artifactOriginDateOridinal = reader.GetOrdinal("a_origin_date");
                    var institutionIdOridinal = reader.GetOrdinal("i_institution_id");
                    var institutionNameOridinal = reader.GetOrdinal("i_name");
                    var institutionFoundedYearOridinal = reader.GetOrdinal("i_founded_year");
                    var assignmentRoleOridinal = reader.GetOrdinal("sa_role");
                    var staffIdOridinal = reader.GetOrdinal("s_staff_id");
                    var staffFirstNameOridinal = reader.GetOrdinal("s_first_name");
                    var staffLastNameOridinal = reader.GetOrdinal("s_last_name");
                    var staffHireDateOridinal = reader.GetOrdinal("s_hire_date");

                    institution = new()
                    {
                        InstitutionId = reader.GetInt32(institutionIdOridinal),
                        Name = reader.GetString(institutionNameOridinal),
                        FoundedYear = reader.GetInt32(institutionFoundedYearOridinal)
                    };

                    artifact = new()
                    {
                        ArtifactId = reader.GetInt32(artifactIdOridinal),
                        Name = reader.GetString(artifactNameOridinal),
                        OriginDate = reader.GetDateTime(artifactOriginDateOridinal),
                        Institution = institution
                    };

                    project = new()
                    {
                        Artifact = artifact,
                        ProjectId = reader.GetInt32(projectIdOridinal),
                        StartDate = reader.GetDateTime(projectStartDateOridinal),
                        EndDate = !await reader.IsDBNullAsync(projectEndDateOridinal) ? reader.GetDateTime(projectEndDateOridinal) : null,
                        Objective = reader.GetString(projectObjectiveOridinal)
                    };

                    do
                    {
                        if (!await reader.IsDBNullAsync(staffLastNameOridinal))
                        {
                            project.StaffAssignments.Add
                            (
                                new()
                                {
                                    FirstName = reader.GetString(staffFirstNameOridinal),
                                    LastName = reader.GetString(staffLastNameOridinal),
                                    HireDate = reader.GetDateTime(staffHireDateOridinal),
                                    Role = reader.GetString(assignmentRoleOridinal)
                                }
                            );
                        }
                    }
                    while (await reader.ReadAsync());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new ArgumentNullException(message: "No entity with given id was found.", e);
            }
        }

        return project;
    }

    public async Task AddNewArtifactAsync(ArtifactPostRequestDTO artifact)
    {
        if (artifact.Artifact.Name.Length > 150) throw new ArgumentException(message: "Artifact.Name cannot be longer than 150 characters.");
        else if (artifact.Project.Objective.Length > 200) throw new ArgumentException(message: "Project.Objective cannot be longer than 200 characters.");

        string commandStr = INSERT_NEW_ARTIFACT_QUERRY + INSERT_NEW_PRESERVATION_PROJECT_QUERRY;

        using (SqlConnection connection = new(this.connectionString))
        using (SqlCommand command = new(commandStr, connection))
        {
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();
            command.Transaction = transaction as SqlTransaction;

            try
            {
                command.Parameters.AddWithValue("@artifact_id", artifact.Artifact.ArtifactId);
                command.Parameters.AddWithValue("@artifact_name", artifact.Artifact.Name);
                command.Parameters.AddWithValue("@artifact_origin_date", artifact.Artifact.OriginDate);
                command.Parameters.AddWithValue("@artifact_institution_id", artifact.Artifact.InstitutionId);
                command.Parameters.AddWithValue("@project_id", artifact.Project.ProjectId);
                command.Parameters.AddWithValue("@project_start_date", artifact.Project.StartDate);
                command.Parameters.AddWithValue("@project_end_date", artifact.Project.EndDate is not null ? artifact.Project.EndDate : DBNull.Value);
                command.Parameters.AddWithValue("@project_objective", artifact.Project.Objective);

                await command.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
            catch (DbException)
            {
                await transaction.RollbackAsync();
                throw new ArgumentException(message: "Entity with given id already exists.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await transaction.RollbackAsync();

                throw new Exception(e.Message, e);
            }
        }
    }
}
