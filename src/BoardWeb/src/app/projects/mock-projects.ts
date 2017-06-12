import { Project, ProjectType } from './project';

export const PROJECTS: Project[] = [
	{
		id: '1',
		name: 'Sample project 1', 
		description: 'Curabitur pulvinar nibh et eros auctor convallis. Sed euismod non massa eu.', 
		projectType: ProjectType.Kanban, 
		members: ['Abigail', 'Bob', 'Cecile', 'Dan'],
		isCompleted: false
	},
	{
		id: '2',
		name: 'Sample project 2', 
		description: 'Curabitur pulvinar nibh et eros auctor convallis. Sed euismod non massa eu.', 
		projectType: ProjectType.Kanban, 
		members: ['Abigail', 'Bob', 'Cecile', 'Dan'],
		isCompleted: false 
	},
	{
		id: '3',
		name: 'Sample project 3', 
		description: 'Curabitur pulvinar nibh et eros auctor convallis. Sed euismod non massa eu.', 
		projectType: ProjectType.Scrum, 
		members: ['Abigail', 'Dan'],
		isCompleted: false 
	},
	{
		id: '4',
		name: 'Sample project 4', 
		description: 'Curabitur pulvinar nibh et eros auctor convallis. Sed euismod non massa eu.', 
		projectType: ProjectType.Kanban, 
		members: ['Abigail', 'Bob', 'Cecile', 'Dan'],
		isCompleted: false 
	},
	{
		id: '5',
		name: 'Sample project 5', 
		description: 'Curabitur pulvinar nibh et eros auctor convallis. Sed euismod non massa eu.', 
		projectType: ProjectType.Kanban, 
		members: ['Abigail', 'Bob', 'Cecile', 'Dan'],
		isCompleted: true 
	},
	{
		id: '6',
		name: 'Sample project 6', 
		description: 'Curabitur pulvinar nibh et eros auctor convallis. Sed euismod non massa eu.', 
		projectType: ProjectType.Scrum, 
		members: ['Abigail', 'Cecile', 'Dan'],
		isCompleted: true 
	},
];