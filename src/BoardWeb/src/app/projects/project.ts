export enum ProjectType {
	Kanban,
	Scrum
};

export class Project {
	projectId: string;
	projectName: string;
	projectAbbreviation: string;
	description: string;
	projectType: ProjectType;
	members: string[];
	isCompleted: boolean;
};