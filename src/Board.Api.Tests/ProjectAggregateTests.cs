using System;
using System.Linq;
using System.Reflection;
using Board.Api.Domain;
using Board.Api.Domain.Events;
using Xunit;

namespace Board.Api.Tests
{
    public class ProjectAggregateTests
    {
        [Fact]
        public void Creating_a_project_with_valid_data_should_create_a_valid_project_and_publish_a_ProjectCreated_event()
        {
            var subject = new Project();
            subject.Create("Teszt project", "TP", "Desc", ProjectType.Kanban);

            Assert.True(subject.IsValid);
            Assert.Contains(subject.GetUncommitedEvents(), ev => ev.GetType().IsAssignableFrom(typeof(ProjectCreated)));

            var createdEvent = subject.GetUncommitedEvents()
                .Single(ev => ev.GetType().IsAssignableFrom(typeof(ProjectCreated))) as ProjectCreated;

            Assert.NotEqual(Guid.Empty, createdEvent.ProjectId);
            Assert.Equal("Teszt project", createdEvent.ProjectName);
            Assert.Equal("TP", createdEvent.ProjectAbbreviation);
            Assert.Equal("Desc", createdEvent.Description);
            Assert.Equal(ProjectType.Kanban, createdEvent.ProjectType);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Creating_a_project_with_empty_name_should_be_invalid(string projectName)
        {
            var subject = new Project();
            subject.Create(projectName, "TP", "Desc", ProjectType.Kanban);

            Assert.False(subject.IsValid);
            Assert.Empty(subject.GetUncommitedEvents());
            Assert.Contains(subject.ViolatedRules, rule => rule.Contains("project name"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Creating_a_project_with_empty_project_abbreviation_should_be_invalid(string projectAbbreviation)
        {
            var subject = new Project();
            subject.Create("Project", projectAbbreviation, "Desc", ProjectType.Kanban);

            Assert.False(subject.IsValid);
            Assert.Empty(subject.GetUncommitedEvents());
            Assert.Contains(subject.ViolatedRules, rule => rule.Contains("project abbreviation"));
        }

        [Fact]
        public void A_newly_created_project_should_not_be_completed()
        {
            var subject = new Project();
            subject.Create("Teszt project", "TP", "", ProjectType.Kanban);

            Assert.True(subject.IsValid);
            Assert.False(subject.IsCompleted, "A new project should not be completed.");
        }

        [Fact]
        public void Creating_an_already_created_project_should_be_invalid()
        {
            var subject = new Project();
            subject.ApplyEvent<ProjectCreated>(new ProjectCreated(Guid.NewGuid(), "Teszt project", "TP", "old description", ProjectType.Scrum));
            subject.Create("Teszt project", "TP", "", ProjectType.Scrum);
            
            Assert.False(subject.IsValid);
            Assert.Contains(subject.ViolatedRules, rule => rule.Contains("already created"));
        }

        [Fact]
        public void Changing_the_project_description_should_publish_a_ProjectDescriptionChanged_event()
        {
            var project = new Project();
            project.ApplyEvent<ProjectCreated>(new ProjectCreated(Guid.NewGuid(), "Teszt project", "TP", "old description", ProjectType.Scrum));
            project.ChangeDescription("new description");

            Assert.True(project.IsValid);
            Assert.Single(project.GetUncommitedEvents(),
                ev => ev.GetType().IsAssignableFrom(typeof(ProjectDescriptionChanged)));
            var subject =
                project.GetUncommitedEvents()
                    .Single(ev => ev.GetType()
                        .IsAssignableFrom(typeof(ProjectDescriptionChanged))) as ProjectDescriptionChanged;
            Assert.Equal("old description", subject.OldDescription);
            Assert.Equal("new description", subject.NewDescription);
        }

        [Fact]
        public void Changing_the_project_description_to_the_same_description_should_not_publish_a_ProjectDescriptionChanged_event()
        {
            var project = new Project();
            project.ApplyEvent<ProjectCreated>(new ProjectCreated(Guid.NewGuid(), "Teszt project", "TP", "old description", ProjectType.Scrum));
            project.ChangeDescription("old description");

            Assert.True(project.IsValid);
            Assert.DoesNotContain(project.GetUncommitedEvents(),
                ev => ev.GetType().IsAssignableFrom(typeof(ProjectDescriptionChanged)));
        }

        [Fact]
        public void Changing_the_project_type_should_publish_a_ProjectTypeChanged_event()
        {
            var project = new Project();
            project.ApplyEvent<ProjectCreated>(new ProjectCreated(Guid.NewGuid(), "Teszt project", "TP", "old description", ProjectType.Scrum));
            project.ChangeProjectType(ProjectType.Kanban);

            Assert.True(project.IsValid);
            Assert.Single(project.GetUncommitedEvents(),
                ev => ev.GetType().IsAssignableFrom(typeof(ProjectTypeChanged)));
            var subject =
                project.GetUncommitedEvents()
                    .Single(ev => ev.GetType()
                        .IsAssignableFrom(typeof(ProjectTypeChanged))) as ProjectTypeChanged;

            Assert.Equal(ProjectType.Scrum, subject.OldProjectType);
            Assert.Equal(ProjectType.Kanban, subject.NewProjectType);
        }
    }
}

