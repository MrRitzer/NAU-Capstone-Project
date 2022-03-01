import { Contact } from './Contact';
import { Links } from './Links';
export class GetManyResponse {
    contacts : Array<Contact>;
    contacts_count : number;
    _links : Links;
    status : string;
}