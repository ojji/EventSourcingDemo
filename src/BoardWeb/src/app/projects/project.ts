export enum ProjectType {
	Kanban,
	Scrum
};

export class Project {
	id: string;
	name: string;
	description: string;
	projectType: ProjectType;
	members: string[];
	isCompleted: boolean;
};