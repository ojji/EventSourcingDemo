export enum ProjectType {
	Kanban,
	Scrum
};

export class Project {
	projectId: string;
	projectName: string;
	description: string;
	projectType: ProjectType;
	members: string[];
	isCompleted: boolean;
};