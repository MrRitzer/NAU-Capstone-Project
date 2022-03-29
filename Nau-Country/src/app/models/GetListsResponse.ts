import { Links } from "./Links"
import { ContactList } from "./ContactList";

export class GetListsResponse
{
        lists: Array<ContactList>;
        lists_count: BigInteger;
        _links: Links;
}
