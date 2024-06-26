import {
  differenceInHours,
  differenceInMinutes,
  differenceInSeconds,
  format,
  parseISO,
} from "date-fns";

export const convertUTCToLocalTime = (utcString: string): string => {
  const date = parseISO(utcString);
  const localDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000);
  return format(localDate, "yyyy-MM-dd HH:mm:ss");
};

export const calculateTimeLeft = (
  endDate: string
): { hours: number; minutes: number; seconds: number } => {
  const end = parseISO(endDate);
  const now = new Date();
  const hours = differenceInHours(end, now);
  const minutes = differenceInMinutes(end, now) % 60;
  const seconds = differenceInSeconds(end, now) % 60;
  return { hours, minutes, seconds };
};
