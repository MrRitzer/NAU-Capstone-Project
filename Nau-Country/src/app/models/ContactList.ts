import { Contact } from "./Contact";

export class ContactList{
  list_id: string;
  name: string;
  description: string;
  favorite: boolean;
  created_at: string;
  updated_at: string;
  membership_count: BigInteger;
  contacts: Contact[];
}

