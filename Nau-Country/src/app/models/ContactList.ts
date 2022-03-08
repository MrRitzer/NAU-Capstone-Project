import { Contact } from "./Contact";

export class ContactList{
  list_id: string;
  name: string;
  description: string;
  favorite: boolean;
  created_at: Date;
  updated_at: Date;
  contacts: Contact[];
}

