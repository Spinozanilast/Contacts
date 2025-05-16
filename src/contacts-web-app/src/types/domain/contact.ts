export type Contact = {
  id: string;
  name: string;
  mobilePhone: string;
  created: string;
  jobTitle?: string | null;
  birthDate?: string | null;
  lastModified?: string | null;
};

export type ContactsParams = {
  pageNumber: number;
  pageSize: number;
  nameSearch?: string;
  jobTitleSearch?: string;
};

export type PagedContactsResponse = {
  data: Contact[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
};

export type ContactsResponse = {
  contacts: PagedContactsResponse;
  jobs: string[];
};

export type SaveContactDto = {
  name: string;
  mobilePhone: string;
  jobTitle?: string;
  birthDate?: string;
};

export type DeleteContactDto = {
  id: string;
};

export interface UpdateContactDto extends SaveContactDto {
  id: string;
}
