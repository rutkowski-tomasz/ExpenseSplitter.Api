import { Card, CardContent, CardHeader } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import { Calendar, DollarSign, Users } from 'lucide-react';

interface Expense {
  id: string;
  name: string;
  amount: number;
  paidBy: string;
  paidByName: string;
  date: string;
  participantCount: number;
  userShare: number;
}

interface ExpenseCardProps {
  expense: Expense;
  onClick: () => void;
}

export function ExpenseCard({ expense, onClick }: ExpenseCardProps) {
  return (
    <Card 
      className="shadow-card hover:shadow-card-hover transition-all duration-300 border-0 cursor-pointer group"
      onClick={onClick}
    >
      <CardHeader className="pb-3">
        <div className="flex items-start justify-between">
          <div className="flex-1">
            <h3 className="font-semibold text-card-foreground group-hover:text-primary transition-colors">
              {expense.name}
            </h3>
            <div className="flex items-center gap-2 mt-2 text-sm text-muted-foreground">
              <Calendar className="w-4 h-4" />
              <span>{expense.date}</span>
            </div>
          </div>
          
          <div className="text-right">
            <div className="font-bold text-lg">${expense.amount.toFixed(2)}</div>
            <div className="text-sm text-muted-foreground">Total</div>
          </div>
        </div>
      </CardHeader>
      
      <CardContent className="space-y-3">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2">
            <Avatar className="w-8 h-8">
              <AvatarFallback className="text-xs bg-primary-light text-primary">
                {expense.paidByName.charAt(0).toUpperCase()}
              </AvatarFallback>
            </Avatar>
            <div>
              <div className="text-sm font-medium">Paid by {expense.paidByName}</div>
              <div className="flex items-center gap-1 text-xs text-muted-foreground">
                <Users className="w-3 h-3" />
                <span>{expense.participantCount} people</span>
              </div>
            </div>
          </div>
          
          <div className="text-right">
            <div className="text-sm text-muted-foreground">Your share</div>
            <Badge 
              variant="secondary"
              className="font-medium bg-primary-light text-primary rounded-full"
            >
              ${expense.userShare.toFixed(2)}
            </Badge>
          </div>
        </div>
      </CardContent>
    </Card>
  );
}