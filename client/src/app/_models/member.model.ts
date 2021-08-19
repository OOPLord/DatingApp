import { Photo } from "./photo.model";

export interface Member {
    id: number;
    userName: string;
    photoUrl: string;
    age: string;
    knownAs: string;
    created: Date;
    lastActive: Date;
    gender: string;
    introduction: string;
    interests: string;
    city: string;
    country: string;
    photos: Photo[];
}