import { Announcement } from '../../announcements/models/announcement.model';
import { Project } from '../../projects/models/project.model';

export interface IEmployeeDashboard {
  presentDaysThisMonth: number;
  leaveCountThisMonth: number;
  announcements: Announcement[];
  assignedProjects: Project[];
}
