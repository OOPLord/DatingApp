import { User } from "./user.model";

export class UserParams {
    gender: string;
    minAge = 18;
    maxAge = 250;
    pageNumber = 1;
    pageSize = 5;
    orderBy = 'lastActive';

    constructor(user: User) {
        this.gender = user.gender === 'female' ? 'male' : 'female'; 
    }
}