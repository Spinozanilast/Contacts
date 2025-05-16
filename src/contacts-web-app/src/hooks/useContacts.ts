import { useMutation, useQuery } from "@tanstack/react-query";
import toast from "react-hot-toast";
import type {
  Contact,
  ContactsResponse,
  ContactsParams,
  SaveContactDto,
  UpdateContactDto,
} from "@/types/domain/contact";
import { api } from "@/lib/api";

export function useContacts(params: ContactsParams) {
  return useQuery({
    queryKey: ["contacts", params],
    queryFn: async () => {
      const response = await api.get<ContactsResponse>("/contacts", {
        params: {
          pageNumber: params.pageNumber,
          pageSize: params.pageSize,
          nameSearch: params.nameSearch,
          jobTitleSearch: params.jobTitleSearch,
        },
      });
      return response.data;
    },
  });
}

export function useContact(id: string) {
  return useQuery({
    queryKey: ["contact", id],
    queryFn: async () => {
      const response = await api.get<Contact>(`/contacts/${id}`);
      return response.data;
    },
    enabled: !!id,
  });
}

export function useCreateContact() {
  return useMutation({
    mutationFn: async (contact: SaveContactDto) => {
      const response = await api.post<void>("/contacts", contact);
      return response.data;
    },
    onSuccess: () => {
      toast.success("Contact created successfully");
    },
    onError: (error) => {
      console.error("Error creating contact:", error);
      toast.error("Failed to create contact");
    },
  });
}

export function useUpdateContact() {
  return useMutation({
    mutationFn: async (contact: UpdateContactDto) => {
      const { id, ...data } = contact;
      const response = await api.put<void>(`/contacts/${id}`, data);
      return response.data;
    },
    onSuccess: () => {
      toast.success("Contact updated successfully");
    },
    onError: (error) => {
      console.error("Error updating contact:", error);
      toast.error("Failed to update contact");
    },
  });
}

export function useDeleteContact() {
  return useMutation({
    mutationFn: async (id: string) => {
      const response = await api.delete<void>(`/contacts/${id}`);
      return response.data;
    },
    onSuccess: () => {
      toast.success("Contact deleted successfully");
    },
    onError: (error) => {
      console.error("Error deleting contact:", error);
      toast.error("Failed to delete contact");
    },
  });
}
