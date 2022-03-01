import { EmailAddress } from "./EmailAddress";
import { CustomField } from "./CustomField";
import { PhoneNumber } from "./PhoneNumber";
import { StreetAddress } from "./StreetAddress";
import { Note } from "./Note";

export class Contact {
    contact_id : string;
    email_address : EmailAddress;
    first_name : string;
    last_name : string;
    job_title : string;
    company_name : string;
    birthday_month : number;
    birthday_day : number;
    anniversary : string;
    update_source : string;
    create_source : string;
    created_at : Date;
    updated_at : Date;
    deleted_at : string;
    custom_fields : Array<CustomField>;
    phone_numbers : Array<PhoneNumber>;
    street_addresses : Array<StreetAddress>;
    list_memberships : Array<string>;
    taggins : Array<string>;
    notes : Array<Note>;
}