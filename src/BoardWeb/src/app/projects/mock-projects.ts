import { Project, ProjectType } from './project';

export const PROJECTS: Project[] = [
	{
		projectId: '1',
		projectName: 'Sample project 1', 
		projectAbbreviation: 'SPONE',
		description: 'Curabitur pulvinar nibh et eros auctor convallis. Sed euismod non massa eu.', 
		projectType: ProjectType.Kanban, 
		members: ['Abigail', 'Bob', 'Cecile', 'Dan'],
		isCompleted: false
	},
	{
		projectId: '2',
		projectName: 'Sample project 2', 
		projectAbbreviation: 'SPTWO',
		description: 'Curabitur pulvinar nibh et eros auctor convallis. Sed euismod non massa eu.', 
		projectType: ProjectType.Kanban, 
		members: ['Abigail', 'Bob', 'Cecile', 'Dan'],
		isCompleted: false 
	},
	{
		projectId: '3',
		projectName: 'Sample project 3', 
		projectAbbreviation: 'SPTHREE',
		description: 'Curabitur pulvinar nibh et eros auctor convallis. Sed euismod non massa eu.', 
		projectType: ProjectType.Scrum, 
		members: ['Abigail', 'Dan'],
		isCompleted: false 
	},
	{
		projectId: '4',
		projectName: 'Sample project 4', 
		projectAbbreviation: 'SPFOUR',
		description: 'Curabitur pulvinar nibh et eros auctor convallis. Sed euismod non massa eu.', 
		projectType: ProjectType.Kanban, 
		members: ['Abigail', 'Bob', 'Cecile', 'Dan'],
		isCompleted: false 
	},
	{
		projectId: '5',
		projectName: 'Sample project 5', 
		projectAbbreviation: 'SPFIVE',
		description: 'Curabitur pulvinar nibh et eros auctor convallis. Sed euismod non massa eu.', 
		projectType: ProjectType.Kanban, 
		members: ['Abigail', 'Bob', 'Cecile', 'Dan'],
		isCompleted: true 
	},
	{
		projectId: '6',
		projectName: 'Sample project 6', 
		projectAbbreviation: 'SPSIX',
		description: 'Curabitur pulvinar nibh et eros auctor convallis. Sed euismod non massa eu.', 
		projectType: ProjectType.Scrum, 
		members: ['Abigail', 'Cecile', 'Dan'],
		isCompleted: true 
	},
];