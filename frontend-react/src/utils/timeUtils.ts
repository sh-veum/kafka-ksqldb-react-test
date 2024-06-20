import { format, parseISO } from "date-fns";

export const convertUTCToLocalTime = (utcString: string): string => {
  const date = parseISO(utcString);
  const localDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
  return format(localDate, "yyyy-MM-dd HH:mm:ss");
};
