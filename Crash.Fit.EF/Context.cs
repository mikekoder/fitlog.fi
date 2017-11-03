using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Crash.Fit.EF.Nutrition;
using Crash.Fit.EF.Training;
using F=Crash.Fit.EF.Feedback;
using Crash.Fit.EF.Measurements;
using Crash.Fit.EF.Logging;
using System.Linq;

namespace Crash.Fit.EF
{
    public partial class Context : Microsoft.EntityFrameworkCore.DbContext
    {
        public virtual DbSet<DailyIntake> DailyIntakes { get; set; }
        public virtual DbSet<Exercise> Exercises { get; set; }
        public virtual DbSet<ExerciseTarget> ExerciseTargets { get; set; }
        public virtual DbSet<F.Feedback> Feedbacks { get; set; }
        public virtual DbSet<F.FeedbackComment> FeedbackComments { get; set; }
        public virtual DbSet<F.FeedbackVote> FeedbackVotes { get; set; }
        public virtual DbSet<Food> Foods { get; set; }
        public virtual DbSet<FoodNutrient> FoodNutrients { get; set; }
        public virtual DbSet<FoodPortion> FoodPortions { get; set; }
        public virtual DbSet<LogException> LogExceptions { get; set; }
        public virtual DbSet<Meal> Meals { get; set; }
        public virtual DbSet<MealDefinition> MealDefinitions { get; set; }
        public virtual DbSet<MealNutrient> MealNutrients { get; set; }
        public virtual DbSet<MealRow> MealRows { get; set; }
        public virtual DbSet<MealRowNutrient> MealRowNutrients { get; set; }
        public virtual DbSet<Measure> Measures { get; set; }
        public virtual DbSet<Measurement> Measurements { get; set; }
        public virtual DbSet<MuscleGroup> MuscleGroups { get; set; }
        public virtual DbSet<Nutrient> Nutrients { get; set; }
        public virtual DbSet<NutrientSettings> NutrientSettings { get; set; }
        public virtual DbSet<NutritionGoal> NutritionGoals { get; set; }
        public virtual DbSet<NutritionGoalMeal> NutritionGoalMeals { get; set; }
        public virtual DbSet<NutritionGoalPeriod> NutritionGoalPeriods { get; set; }
        public virtual DbSet<NutritionGoalValue> NutritionGoalValues { get; set; }
        public virtual DbSet<OneRepMax> OneRepMaxs { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public virtual DbSet<Routine> Routines { get; set; }
        public virtual DbSet<RoutineExercise> RoutineExercises { get; set; }
        public virtual DbSet<RoutineWorkout> RoutineWorkouts { get; set; }
        public virtual DbSet<TrainingGoal> TrainingGoals { get; set; }
        public virtual DbSet<TrainingGoalExercise> TrainingGoalExercises { get; set; }
        public virtual DbSet<Workout> Workouts { get; set; }
        public virtual DbSet<WorkoutSet> WorkoutSets { get; set; }

        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DailyIntake>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.EndAge).HasColumnType("decimal(4, 1)");

                entity.Property(e => e.MaxAmount).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.MinAmount).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.StartAge).HasColumnType("decimal(4, 1)");

                entity.HasOne(d => d.Nutrient)
                    .WithMany(p => p.DailyIntakes)
                    .HasForeignKey(d => d.NutrientId)
                    .HasConstraintName("FK_DailyIntake_Nutrient");
            });

            modelBuilder.Entity<Exercise>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Exercises)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Exercise_Profile");
            });

            modelBuilder.Entity<ExerciseTarget>(entity =>
            {
                entity.HasKey(e => new { e.ExerciseId, e.MuscleGroupId });

                entity.HasOne(d => d.Exercise)
                    .WithMany(p => p.Targets)
                    .HasForeignKey(d => d.ExerciseId)
                    .HasConstraintName("FK_ExerciseTarget_Exercise");

                entity.HasOne(d => d.MuscleGroup)
                    .WithMany(p => p.Exercises)
                    .HasForeignKey(d => d.MuscleGroupId)
                    .HasConstraintName("FK_ExerciseTarget_MuscleGroup");
            });

            modelBuilder.Entity<F.Feedback>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Feedback_Profile");
            });

            modelBuilder.Entity<F.FeedbackComment>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Content).IsRequired();

                entity.HasOne(d => d.Feedback)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.FeedbackId)
                    .HasConstraintName("FK_FeedbackComment_Feedback");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FeedbackComments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_FeedbackComment_Profile");
            });

            modelBuilder.Entity<F.FeedbackVote>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Feedback)
                    .WithMany(p => p.Votes)
                    .HasForeignKey(d => d.FeedbackId)
                    .HasConstraintName("FK_FeedbackVote_Feedback");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FeedbackVotes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_FeedbackVote_Profile");
            });

            modelBuilder.Entity<Food>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CookedWeight).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.FineliId).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Foods)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Food_Profile");
            });

            modelBuilder.Entity<FoodNutrient>(entity =>
            {
                entity.HasKey(e => new { e.FoodId, e.NutrientId });

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.PortionAmount).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.Food)
                    .WithMany(p => p.Nutrients)
                    .HasForeignKey(d => d.FoodId)
                    .HasConstraintName("FK_FoodNutrient_Food");

                entity.HasOne(d => d.Nutrient)
                    .WithMany(p => p.FoodNutrient)
                    .HasForeignKey(d => d.NutrientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FoodNutrient_Nutrient");
            });

            modelBuilder.Entity<FoodPortion>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Weight).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.Food)
                    .WithMany(p => p.Portions)
                    .HasForeignKey(d => d.FoodId)
                    .HasConstraintName("FK_FoodPortion_Food");
            });

            modelBuilder.Entity<LogException>(entity =>
            {
                entity.Property(e => e.Method).HasMaxLength(10);

                entity.Property(e => e.Path).HasMaxLength(500);
            });

            modelBuilder.Entity<Meal>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Definition)
                    .WithMany(p => p.Meal)
                    .HasForeignKey(d => d.DefinitionId)
                    .HasConstraintName("FK_Meal_MealDefinition");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Meals)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Meal_Profile");
            });

            modelBuilder.Entity<MealDefinition>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MealDefinitions)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_MealDefinition_Profile");
            });

            modelBuilder.Entity<MealNutrient>(entity =>
            {
                entity.HasKey(e => new { e.MealId, e.NutrientId });

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.Meal)
                    .WithMany(p => p.Nutrients)
                    .HasForeignKey(d => d.MealId)
                    .HasConstraintName("FK_MealNutrient_Meal");

                entity.HasOne(d => d.Nutrient)
                    .WithMany(p => p.MealNutrient)
                    .HasForeignKey(d => d.NutrientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MealNutrient_Nutrient");
            });

            modelBuilder.Entity<MealRow>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Quantity).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Weight).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.Food)
                    .WithMany(p => p.MealRow)
                    .HasForeignKey(d => d.FoodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MealRow_Food");

                entity.HasOne(d => d.Meal)
                    .WithMany(p => p.Rows)
                    .HasForeignKey(d => d.MealId)
                    .HasConstraintName("FK_MealRow_Meal");

                entity.HasOne(d => d.Portion)
                    .WithMany(p => p.MealRow)
                    .HasForeignKey(d => d.PortionId)
                    .HasConstraintName("FK_MealRow_FoodPortion");
            });

            modelBuilder.Entity<MealRowNutrient>(entity =>
            {
                entity.HasKey(e => new { e.MealRowId, e.NutrientId });

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.MealRow)
                    .WithMany(p => p.Nutrients)
                    .HasForeignKey(d => d.MealRowId)
                    .HasConstraintName("FK_MealRowNutrient_MealRow");

                entity.HasOne(d => d.Nutrient)
                    .WithMany(p => p.MealRowNutrient)
                    .HasForeignKey(d => d.NutrientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MealRowNutrient_Nutrient");
            });

            modelBuilder.Entity<Measure>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Measures)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Measure_Profile");
            });

            modelBuilder.Entity<Measurement>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Value).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.Measure)
                    .WithMany(p => p.Measurement)
                    .HasForeignKey(d => d.MeasureId)
                    .HasConstraintName("FK_Measurement_Measure");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Measurements)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Measurement_Profile");
            });

            modelBuilder.Entity<MuscleGroup>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Nutrient>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DefaultOrder).HasDefaultValueSql("((0))");

                entity.Property(e => e.FineliClass).HasMaxLength(50);

                entity.Property(e => e.FineliGroup).HasMaxLength(50);

                entity.Property(e => e.FineliId).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ShortName).HasMaxLength(50);

                entity.Property(e => e.Unit)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<NutrientSettings>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Nutrient)
                    .WithMany(p => p.NutrientSettings)
                    .HasForeignKey(d => d.NutrientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NutrientSettings_Nutrient");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.NutrientSettings)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_NutrientSettings_Profile");
            });

            modelBuilder.Entity<NutritionGoal>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.NutritionGoals)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_NutritionGoal_Profile");
            });

            modelBuilder.Entity<NutritionGoalMeal>(entity =>
            {
                entity.HasKey(e => new { e.NutritionGoalPeriodId, e.MealDefinitionId });

                entity.HasOne(d => d.MealDefinition)
                    .WithMany(p => p.NutritionGoalMeal)
                    .HasForeignKey(d => d.MealDefinitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NutritionGoalMeal_MealDefinition");

                entity.HasOne(d => d.NutritionGoalPeriod)
                    .WithMany(p => p.Meals)
                    .HasForeignKey(d => d.NutritionGoalPeriodId)
                    .HasConstraintName("FK_NutritionGoalMeal_NutritionGoalTime");
            });

            modelBuilder.Entity<NutritionGoalPeriod>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.NutritionGoal)
                    .WithMany(p => p.Periods)
                    .HasForeignKey(d => d.NutritionGoalId)
                    .HasConstraintName("FK_NutritionGoalTime_NutritionGoal");
            });

            modelBuilder.Entity<NutritionGoalValue>(entity =>
            {
                entity.HasKey(e => new { e.NutritionGoalPeriodId, e.NutrientId });

                entity.Property(e => e.Max).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Min).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.Nutrient)
                    .WithMany(p => p.NutritionGoalValue)
                    .HasForeignKey(d => d.NutrientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NutritionGoal_Nutrient");

                entity.HasOne(d => d.NutritionGoalPeriod)
                    .WithMany(p => p.Values)
                    .HasForeignKey(d => d.NutritionGoalPeriodId)
                    .HasConstraintName("FK_NutritionGoalValue_NutritionGoalTime");
            });

            modelBuilder.Entity<OneRepMax>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BodyWeightMax).HasColumnType("decimal(8, 3)");

                entity.Property(e => e.Max).HasColumnType("decimal(8, 3)");

                entity.HasOne(d => d.Exercise)
                    .WithMany(p => p.OneRepMax)
                    .HasForeignKey(d => d.ExerciseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OneRepMax_Exercise");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.OneRepMaxs)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_OneRepMax_Profile");
            });

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.DoB).HasColumnType("datetime");

                entity.Property(e => e.Gender).HasMaxLength(50);

                entity.Property(e => e.Height).HasColumnType("decimal(10, 4)");

                entity.Property(e => e.RefreshToken).HasMaxLength(100);

                entity.Property(e => e.Rmr).HasColumnType("decimal(10, 4)");

                entity.Property(e => e.Weight).HasColumnType("decimal(10, 4)");
            });

            modelBuilder.Entity<RecipeIngredient>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Quantity).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Weight).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.Food)
                    .WithMany(p => p.RecipeIngredients)
                    .HasForeignKey(d => d.FoodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecipeIngredient_Food");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipeIngredientRecipe)
                    .HasForeignKey(d => d.RecipeId)
                    .HasConstraintName("FK_RecipeIngredient_Recipe");
            });

            modelBuilder.Entity<Routine>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Routines)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Routine_Profile");
            });

            modelBuilder.Entity<RoutineExercise>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.LoadFrom).HasColumnType("decimal(8, 3)");

                entity.Property(e => e.LoadTo).HasColumnType("decimal(8, 3)");

                entity.HasOne(d => d.Exercise)
                    .WithMany(p => p.RoutineExercise)
                    .HasForeignKey(d => d.ExerciseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoutineExercise_Exercise");

                entity.HasOne(d => d.RoutineWorkout)
                    .WithMany(p => p.Exercises)
                    .HasForeignKey(d => d.RoutineWorkoutId)
                    .HasConstraintName("FK_RoutineExercise_RoutineWorkout");
            });

            modelBuilder.Entity<RoutineWorkout>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Frequency).HasColumnType("decimal(8, 3)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Routine)
                    .WithMany(p => p.Workouts)
                    .HasForeignKey(d => d.RoutineId)
                    .HasConstraintName("FK_RoutineWorkout_Routine");
            });

            modelBuilder.Entity<TrainingGoal>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TrainingGoals)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_TrainingGoal_Profile");
            });

            modelBuilder.Entity<TrainingGoalExercise>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Frequency).HasColumnType("decimal(18, 4)");

                entity.HasOne(d => d.Exercise)
                    .WithMany(p => p.TrainingGoalExercise)
                    .HasForeignKey(d => d.ExerciseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrainingGoalExercise_Exercise");

                entity.HasOne(d => d.TrainingGoal)
                    .WithMany(p => p.Exercises)
                    .HasForeignKey(d => d.TrainingGoalId)
                    .HasConstraintName("FK_TrainingGoalExercise_TrainingGoal");
            });

            modelBuilder.Entity<Workout>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Workouts)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Workout_Profile");
            });

            modelBuilder.Entity<WorkoutSet>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BodyWeightLoad).HasColumnType("decimal(8, 3)");

                entity.Property(e => e.Load).HasColumnType("decimal(8, 3)");

                entity.Property(e => e.Weights).HasColumnType("decimal(8, 3)");

                entity.HasOne(d => d.Exercise)
                    .WithMany(p => p.WorkoutSet)
                    .HasForeignKey(d => d.ExerciseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkoutSet_Exercise");

                entity.HasOne(d => d.Workout)
                    .WithMany(p => p.Sets)
                    .HasForeignKey(d => d.WorkoutId)
                    .HasConstraintName("FK_WorkoutSet_Workout");
            });

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SqlServer().TableName = entity.ClrType.Name;
            }
        }
    }
}
