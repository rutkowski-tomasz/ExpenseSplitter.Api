import { Card, CardContent, CardHeader } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Users, Calendar, DollarSign, ArrowRight } from 'lucide-react';

interface Settlement {
  id: string;
  name: string;
  participantCount: number;
  totalExpenses: number;
  lastActivity: string;
  userBalance: number;
}

interface SettlementCardProps {
  settlement: Settlement;
  onClick: () => void;
}

export function SettlementCard({ settlement, onClick }: SettlementCardProps) {
  const isOwed = settlement.userBalance > 0;
  const isOwing = settlement.userBalance < 0;

  return (
    <Card 
      className="shadow-card hover:shadow-card-hover transition-all duration-300 border-0 cursor-pointer group"
      onClick={onClick}
    >
      <CardHeader className="pb-3">
        <div className="flex items-center justify-between">
          <h3 className="font-semibold text-lg text-card-foreground group-hover:text-primary transition-colors">
            {settlement.name}
          </h3>
          <ArrowRight className="w-5 h-5 text-muted-foreground group-hover:text-primary group-hover:translate-x-1 transition-all duration-300" />
        </div>
        
        <div className="flex items-center gap-4 text-sm text-muted-foreground">
          <div className="flex items-center gap-1">
            <Users className="w-4 h-4" />
            <span>{settlement.participantCount} people</span>
          </div>
          <div className="flex items-center gap-1">
            <Calendar className="w-4 h-4" />
            <span>{settlement.lastActivity}</span>
          </div>
        </div>
      </CardHeader>
      
      <CardContent className="space-y-3">
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-2">
            <DollarSign className="w-4 h-4 text-muted-foreground" />
            <span className="text-sm text-muted-foreground">Total expenses</span>
          </div>
          <span className="font-medium">${settlement.totalExpenses.toFixed(2)}</span>
        </div>
        
        {settlement.userBalance !== 0 && (
          <div className="flex items-center justify-between">
            <span className="text-sm text-muted-foreground">Your balance</span>
            <Badge 
              variant={isOwed ? "default" : "destructive"}
              className={`
                font-medium rounded-full px-3 py-1
                ${isOwed ? 'bg-green-100 text-green-700 hover:bg-green-200' : ''}
                ${isOwing ? 'bg-red-100 text-red-700 hover:bg-red-200' : ''}
              `}
            >
              {isOwed ? '+' : ''}${settlement.userBalance.toFixed(2)}
            </Badge>
          </div>
        )}
      </CardContent>
    </Card>
  );
}