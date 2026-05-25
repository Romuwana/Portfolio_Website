export interface Certification {
  id?: number;
  name: string;
  issuer?: string;
  dateIssued?: string | Date;
  imageUrl?: string;
  year?: string | number;
}